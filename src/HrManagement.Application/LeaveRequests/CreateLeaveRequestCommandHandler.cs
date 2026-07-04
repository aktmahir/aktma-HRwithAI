using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Leave;
using MediatR;

namespace HrManagement.Application.LeaveRequests;

public sealed class CreateLeaveRequestCommandHandler(
    IRepository<LeaveRequest> leaveRequests,
    IUnitOfWork unitOfWork,
    IEmailService emailService) : IRequestHandler<CreateLeaveRequestCommand, LeaveRequest>
{
    public async Task<LeaveRequest> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = new LeaveRequest(request.EmployeeId, request.StartDate, request.EndDate, request.Reason);

        await leaveRequests.AddAsync(leaveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            try
            {
                await emailService.SendAsync(
                    to: "hr@example.com",
                    subject: $"New Leave Request from Employee {request.EmployeeId}",
                    body: $"Employee {request.EmployeeId} has submitted a leave request from {request.StartDate} to {request.EndDate}.");
            }
            catch
            {
            }
        }, cancellationToken);

        return leaveRequest;
    }
}
