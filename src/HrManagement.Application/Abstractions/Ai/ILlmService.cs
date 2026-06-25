namespace HrManagement.Application.Abstractions.Ai;

public interface ILlmService
{
    Task<CvScreeningResult> ScreenCvAsync(
        string cvText,
        string jobDescription,
        CancellationToken cancellationToken = default);

    Task<PerformanceReviewAnalysisResult> AnalyzePerformanceReviewAsync(
        string reviewText,
        CancellationToken cancellationToken = default);
}
