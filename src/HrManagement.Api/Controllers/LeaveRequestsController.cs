using System.ComponentModel.DataAnnotations;
using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Pagination;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Application.LeaveRequests;
using HrManagement.Domain.Leave;
using HrManagement.Infrastructure.Metrics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize(Policy = "HrAdmin")]
public sealed class LeaveRequestsController(
    IRepository<LeaveRequest> leaveRequests,
    ISender sender,
    AuditLogger auditLogger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LeaveRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<LeaveRequest>>> GetAll(
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

        var totalCount = results.Count;
        var paged = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var pagedResult = HrManagement.Application.Abstractions.Pagination.PagedResult<LeaveRequest>.Create(paged, totalCount, page, pageSize);

        return Ok(pagedResult);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeaveRequest>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return leaveRequest is null ? NotFound() : Ok(leaveRequest);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LeaveRequest>> Create(
        [FromBody] CreateLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await sender.Send(
            new CreateLeaveRequestCommand(request.EmployeeId, request.StartDate, request.EndDate, request.Reason),
            cancellationToken);

        await auditLogger.LogChangeAsync("Create", "LeaveRequest", User.Identity?.Name, $"Submitted leave request {leaveRequest.Id}");
        BusinessMetrics.LeaveRequestsCreated.WithLabels(leaveRequest.EmployeeId.ToString()).Inc();
        return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ApproveLeaveRequestCommand(id, request.ReviewerEmployeeId, request.Notes), cancellationToken);

        await auditLogger.LogChangeAsync("Approve", "LeaveRequest", User.Identity?.Name, $"Approved leave request {id}");
        BusinessMetrics.LeaveRequestsApproved.WithLabels(request.ReviewerEmployeeId.ToString()).Inc();
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RejectLeaveRequestCommand(id, request.ReviewerEmployeeId, request.Notes), cancellationToken);

        await auditLogger.LogChangeAsync("Reject", "LeaveRequest", User.Identity?.Name, $"Rejected leave request {id}");
        BusinessMetrics.LeaveRequestsRejected.WithLabels(request.ReviewerEmployeeId.ToString()).Inc();
        return NoContent();
    }
}

public sealed record CreateLeaveRequest(
    [Required] Guid EmployeeId,
    [Required] DateOnly StartDate,
    [Required] DateOnly EndDate,
    [Required] [StringLength(500)] string Reason);

public sealed record ReviewLeaveRequest(
    [Required] Guid ReviewerEmployeeId,
    [StringLength(500)] string? Notes);
