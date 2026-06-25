using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed record ApproveLeaveRequestCommand(Guid LeaveRequestId, Guid ReviewerEmployeeId, string? Notes)
    : IRequest;
