using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.Common.Exceptions;
using MediatR;

namespace HrManagement.Application.PerformanceReviews;

public sealed class AnalyzePerformanceReviewCommandHandler(ILlmService llmService)
    : IRequestHandler<AnalyzePerformanceReviewCommand, PerformanceReviewAnalysisResult>
{
    public Task<PerformanceReviewAnalysisResult> Handle(
        AnalyzePerformanceReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewText))
        {
            throw new ValidationException("Review text is required.");
        }

        return llmService.AnalyzePerformanceReviewAsync(request.ReviewText, cancellationToken);
    }
}
