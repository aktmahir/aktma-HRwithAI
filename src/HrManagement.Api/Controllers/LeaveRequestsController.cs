using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Application.LeaveRequests;
using HrManagement.Domain.Leave;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public sealed class LeaveRequestsController(
    IRepository<LeaveRequest> leaveRequests,
    ISender sender,
    AuditLogger auditLogger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveRequest>>> GetAll(
        [FromQuery] Guid? employeeId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] LeaveRequestStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var results = await leaveRequests.ListAsync(cancellationToken);

        if (employeeId.HasValue && employeeId.Value != Guid.Empty)
        {
            results = results.Where(r => r.EmployeeId == employeeId.Value).ToList();
        }

        if (startDate.HasValue)
        {
            results = results.Where(r => r.StartDate >= startDate.Value).ToList();
        }

        if (endDate.HasValue)
        {
            results = results.Where(r => r.EndDate <= endDate.Value).ToList();
        }

        if (status.HasValue)
        {
            results = results.Where(r => r.Status == status.Value).ToList();
        }

        var paged = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Ok(paged);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveRequest>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return leaveRequest is null ? NotFound() : Ok(leaveRequest);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequest>> Create(
        [FromBody] CreateLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await sender.Send(
            new CreateLeaveRequestCommand(request.EmployeeId, request.StartDate, request.EndDate, request.Reason),
            cancellationToken);

        auditLogger.LogChange("Create", "LeaveRequest", User.Identity?.Name, $"Submitted leave request {leaveRequest.Id}");
        return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ApproveLeaveRequestCommand(id, request.ReviewerEmployeeId, request.Notes), cancellationToken);

        auditLogger.LogChange("Approve", "LeaveRequest", User.Identity?.Name, $"Approved leave request {id}");
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RejectLeaveRequestCommand(id, request.ReviewerEmployeeId, request.Notes), cancellationToken);

        auditLogger.LogChange("Reject", "LeaveRequest", User.Identity?.Name, $"Rejected leave request {id}");
        return NoContent();
    }
}

public sealed record CreateLeaveRequest(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate, string Reason);

public sealed record ReviewLeaveRequest(Guid ReviewerEmployeeId, string? Notes);
