using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed record RejectLeaveRequestCommand(Guid LeaveRequestId, Guid ReviewerEmployeeId, string? Notes)
    : IRequest;
