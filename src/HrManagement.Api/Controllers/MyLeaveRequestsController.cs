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
[Route("api/leave-requests/mine")]
[Authorize]
public sealed class MyLeaveRequestsController(
    IRepository<LeaveRequest> leaveRequests,
    ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LeaveRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<LeaveRequest>>> GetMyLeaveRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var all = await leaveRequests.ListAsync(cancellationToken);
        var mine = all.Where(r => r.EmployeeId == userId).ToList();
        var totalCount = mine.Count;
        var paged = mine.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var result = PagedResult<LeaveRequest>.Create(paged, totalCount, page, pageSize);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeaveRequest>> Create(
        [FromBody] CreateLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var leaveRequest = await sender.Send(
            new CreateLeaveRequestCommand(userId, request.StartDate, request.EndDate, request.Reason),
            cancellationToken);

        BusinessMetrics.LeaveRequestsCreated.WithLabels(userId.ToString()).Inc();
        return Ok(leaveRequest);
    }
}
