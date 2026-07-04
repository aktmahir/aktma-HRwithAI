using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Common.Exceptions;
using MediatR;

namespace HrManagement.Application.PerformanceReviews;

public sealed class AnalyzePerformanceReviewCommandHandler(ILlmService llmService, IEmailService emailService)
    : IRequestHandler<AnalyzePerformanceReviewCommand, PerformanceReviewAnalysisResult>
{
    public async Task<PerformanceReviewAnalysisResult> Handle(
        AnalyzePerformanceReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewText))
        {
            throw new ValidationException("Review text is required.");
        }

        try
        {
            var result = await llmService.AnalyzePerformanceReviewAsync(request.ReviewText, cancellationToken);

            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendAsync(
                        to: "hr@example.com",
                        subject: "Performance Review Analysis Completed",
                        body: "Performance review analysis has been completed.");
                }
                catch
                {
                }
            }, cancellationToken);

            return result;
        }
        catch
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendAsync(
                        to: "hr@example.com",
                        subject: "Performance Review Analysis Failed",
                        body: "Performance review analysis failed due to an internal error.");
                }
                catch
                {
                }
            }, cancellationToken);

            throw;
        }
    }
}
