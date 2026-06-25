using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Application.Common.Exceptions;
using HrManagement.Domain.Leave;
using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed class RejectLeaveRequestCommandHandler(
    IRepository<LeaveRequest> leaveRequests,
    IUnitOfWork unitOfWork) : IRequestHandler<RejectLeaveRequestCommand>
{
    public async Task Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(request.LeaveRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId);

        leaveRequest.Reject(request.ReviewerEmployeeId, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
