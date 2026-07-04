using HrManagement.Application.LeaveRequests;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Common.Exceptions;
using HrManagement.Domain.Leave;
using Moq;
using Xunit;

namespace HrManagement.Application.Tests.LeaveRequests;

public class ApproveLeaveRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldApproveLeaveRequest_WhenValidCommand()
    {
        var leaveRequestRepository = new Mock<IRepository<LeaveRequest>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");
        var reviewerId = Guid.NewGuid();

        leaveRequestRepository.Setup(r => r.GetByIdAsync(leaveRequest.Id, default)).ReturnsAsync(leaveRequest);
        var handler = new ApproveLeaveRequestCommandHandler(leaveRequestRepository.Object, unitOfWork.Object, Mock.Of<IEmailService>());
        var command = new ApproveLeaveRequestCommand(leaveRequest.Id, reviewerId, "Approved");

        await handler.Handle(command, default);

        Assert.Equal(LeaveRequestStatus.Approved, leaveRequest.Status);
        Assert.Equal(reviewerId, leaveRequest.ReviewedByEmployeeId);
        leaveRequestRepository.Verify(r => r.GetByIdAsync(leaveRequest.Id, default), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenLeaveRequestDoesNotExist()
    {
        var leaveRequestRepository = new Mock<IRepository<LeaveRequest>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        leaveRequestRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((LeaveRequest?)null);
        var handler = new ApproveLeaveRequestCommandHandler(leaveRequestRepository.Object, unitOfWork.Object, Mock.Of<IEmailService>());
        var command = new ApproveLeaveRequestCommand(Guid.NewGuid(), Guid.NewGuid(), "Approved");

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
