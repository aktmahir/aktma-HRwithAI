using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Leave;
using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed class CreateLeaveRequestCommandHandler(
    IRepository<LeaveRequest> leaveRequests,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateLeaveRequestCommand, LeaveRequest>
{
    public async Task<LeaveRequest> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = new LeaveRequest(request.EmployeeId, request.StartDate, request.EndDate, request.Reason);

        await leaveRequests.AddAsync(leaveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return leaveRequest;
    }
}
