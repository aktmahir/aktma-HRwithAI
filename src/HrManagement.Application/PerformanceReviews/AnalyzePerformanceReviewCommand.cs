using HrManagement.Application.Abstractions.Ai;
using MediatR;

namespace HrManagement.Application.PerformanceReviews;

public sealed record AnalyzePerformanceReviewCommand(string ReviewText)
    : IRequest<PerformanceReviewAnalysisResult>;
