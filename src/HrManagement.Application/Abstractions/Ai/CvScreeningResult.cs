namespace HrManagement.Application.Abstractions.Ai;

public sealed record CvScreeningResult(
    int MatchPercentage,
    IReadOnlyCollection<string> MatchingSkills,
    IReadOnlyCollection<string> MissingSkills,
    string Summary,
    string Recommendation);
