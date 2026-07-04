using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Application.Common.Exceptions;
using HrManagement.Domain.Leave;
using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed class ApproveLeaveRequestCommandHandler(
    IRepository<LeaveRequest> leaveRequests,
    IUnitOfWork unitOfWork,
    IEmailService emailService) : IRequestHandler<ApproveLeaveRequestCommand>
{
    public async Task Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequests.GetByIdAsync(request.LeaveRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId);

        leaveRequest.Approve(request.ReviewerEmployeeId, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            try
            {
                await emailService.SendAsync(
                    to: "hr@example.com",
                    subject: $"Leave Request Approved - {request.LeaveRequestId}",
                    body: $"Leave request {request.LeaveRequestId} has been approved by {request.ReviewerEmployeeId}.");
            }
            catch
            {
            }
        }, cancellationToken);
    }
}
