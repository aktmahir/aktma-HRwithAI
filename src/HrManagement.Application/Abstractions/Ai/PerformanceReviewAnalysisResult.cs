namespace HrManagement.Application.Abstractions.Ai;

public sealed record PerformanceReviewAnalysisResult(
    string Sentiment,
    IReadOnlyCollection<string> Strengths,
    IReadOnlyCollection<string> Weaknesses,
    string Summary);
