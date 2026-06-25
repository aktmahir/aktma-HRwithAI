using HrManagement.Domain.Leave;
using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed record CreateLeaveRequestCommand(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason) : IRequest<LeaveRequest>;
