using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Leave;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
public sealed class LeaveRequestsController(
    IRepository<LeaveRequest> leaveRequests,
    IUnitOfWork unitOfWork) : ControllerBase
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
        var leaveRequest = new LeaveRequest(request.EmployeeId, request.StartDate, request.EndDate, request.Reason);
        await leaveRequests.AddAsync(leaveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (leaveRequest is null)
        {
            return NotFound();
        }

        leaveRequest.Approve(request.ReviewerEmployeeId, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ReviewLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (leaveRequest is null)
        {
            return NotFound();
        }

        leaveRequest.Reject(request.ReviewerEmployeeId, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record CreateLeaveRequest(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate, string Reason);

public sealed record ReviewLeaveRequest(Guid ReviewerEmployeeId, string? Notes);
