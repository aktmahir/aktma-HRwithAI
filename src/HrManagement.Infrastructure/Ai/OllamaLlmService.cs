using System.Net.Http.Json;
using System.Text.Json;
using HrManagement.Application.Abstractions.Ai;
using Microsoft.Extensions.Options;

namespace HrManagement.Infrastructure.Ai;

public sealed class OllamaLlmService(HttpClient httpClient, IOptions<LlmOptions> options) : ILlmService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly LlmOptions _options = options.Value;

    public async Task<CvScreeningResult> ScreenCvAsync(
        string cvText,
        string jobDescription,
        CancellationToken cancellationToken = default)
    {
        var prompt = $@"
You are an expert HR technical recruiter.
Return only valid JSON matching this schema:
{{
  ""matchPercentage"": 0,
  ""matchingSkills"": [""skill""],
  ""missingSkills"": [""skill""],
  ""summary"": ""short assessment"",
  ""recommendation"": ""Interview | Hold | Reject""
}}

Job description:
{jobDescription}

Candidate CV:
{cvText}
";

        return await GenerateStructuredResponseAsync<CvScreeningResult>(prompt, cancellationToken);
    }

    public async Task<PerformanceReviewAnalysisResult> AnalyzePerformanceReviewAsync(
        string reviewText,
        CancellationToken cancellationToken = default)
    {
        var prompt = $@"
You are an HR performance review analyst.
Return only valid JSON matching this schema:
{{
  ""sentiment"": ""Positive | Neutral | Negative | Mixed"",
  ""strengths"": [""strength""],
  ""weaknesses"": [""weakness""],
  ""summary"": ""concise summary""
}}

Peer review text:
{reviewText}
";

        return await GenerateStructuredResponseAsync<PerformanceReviewAnalysisResult>(prompt, cancellationToken);
    }

    private async Task<TResponse> GenerateStructuredResponseAsync<TResponse>(
        string prompt,
        CancellationToken cancellationToken)
    {
        var request = new OllamaGenerateRequest(_options.Model, prompt, Stream: false, Format: "json");

        using var response = await httpClient.PostAsJsonAsync(_options.GeneratePath, request, SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("The LLM provider returned an empty response.");

        return JsonSerializer.Deserialize<TResponse>(payload.Response, SerializerOptions)
            ?? throw new InvalidOperationException("The LLM provider returned JSON that could not be parsed.");
    }

    private sealed record OllamaGenerateRequest(string Model, string Prompt, bool Stream, string Format);

    private sealed record OllamaGenerateResponse(string Response);
}
