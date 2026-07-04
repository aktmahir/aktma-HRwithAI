using HrManagement.Application.PerformanceReviews;
using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Common.Exceptions;
using Moq;
using Xunit;

namespace HrManagement.Application.Tests.PerformanceReviews;

public class AnalyzePerformanceReviewCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenReviewTextIsEmpty()
    {
        var handler = new AnalyzePerformanceReviewCommandHandler(Mock.Of<ILlmService>(), Mock.Of<IEmailService>());
        var command = new AnalyzePerformanceReviewCommand(string.Empty);

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenReviewTextIsWhiteSpace()
    {
        var handler = new AnalyzePerformanceReviewCommandHandler(Mock.Of<ILlmService>(), Mock.Of<IEmailService>());
        var command = new AnalyzePerformanceReviewCommand("   ");

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldCallLlmService_WithValidInput()
    {
        var mockLlmService = new Mock<ILlmService>();
        var handler = new AnalyzePerformanceReviewCommandHandler(mockLlmService.Object, Mock.Of<IEmailService>());
        var command = new AnalyzePerformanceReviewCommand("Great employee");

        await handler.Handle(command, default);

        mockLlmService.Verify(m => m.AnalyzePerformanceReviewAsync("Great employee", default), Times.Once);
    }
}
