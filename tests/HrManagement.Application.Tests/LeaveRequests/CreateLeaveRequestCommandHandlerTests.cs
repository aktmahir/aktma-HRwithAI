using HrManagement.Application.LeaveRequests;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Leave;
using Moq;
using Xunit;

namespace HrManagement.Application.Tests.LeaveRequests;

public class CreateLeaveRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateLeaveRequest_WhenValidCommand()
    {
        // Arrange
        var leaveRequestRepository = new Mock<IRepository<LeaveRequest>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateLeaveRequestCommandHandler(leaveRequestRepository.Object, unitOfWork.Object);
        var command = new CreateLeaveRequestCommand(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        leaveRequestRepository.Verify(r => r.AddAsync(It.Is<LeaveRequest>(lr => 
            lr.EmployeeId == command.EmployeeId && 
            lr.StartDate == command.StartDate && 
            lr.EndDate == command.EndDate && 
            lr.Reason == command.Reason), default), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(command.EmployeeId, result.EmployeeId);
        Assert.Equal(command.StartDate, result.StartDate);
        Assert.Equal(command.EndDate, result.EndDate);
        Assert.Equal(command.Reason, result.Reason);
    }
}