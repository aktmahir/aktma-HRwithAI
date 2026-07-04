using HrManagement.Application.CvScreening;
using HrManagement.Application.Common.Exceptions;
using HrManagement.Application.Abstractions.Ai;
using Moq;
using Xunit;

namespace HrManagement.Application.Tests.CvScreening;

public class ScreenCvCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCvTextIsEmpty()
    {
        // Arrange
        var handler = new ScreenCvCommandHandler(Mock.Of<ILlmService>());
        var command = new ScreenCvCommand(string.Empty, "Some job description");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenJobDescriptionIsEmpty()
    {
        // Arrange
        var handler = new ScreenCvCommandHandler(Mock.Of<ILlmService>());
        var command = new ScreenCvCommand("Some cv text", string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldCallLlmService_WithValidInput()
    {
        // Arrange
        var mockLlmService = new Mock<ILlmService>();
        var handler = new ScreenCvCommandHandler(mockLlmService.Object);
        var command = new ScreenCvCommand("CV text", "Job description");

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        mockLlmService.Verify(m => m.ScreenCvAsync("CV text", "Job description", default), Times.Once);
    }
}