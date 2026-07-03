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
    public async Task<ActionResult<IReadOnlyList<LeaveRequest>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await leaveRequests.ListAsync(cancellationToken));
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
