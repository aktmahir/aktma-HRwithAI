namespace HrManagement.Infrastructure.Ai;

public sealed class LlmOptions
{
    public const string SectionName = "Llm";

    public string BaseUrl { get; init; } = string.Empty;
    public string Model { get; init; } = "llama3";
    public string GeneratePath { get; init; } = "/api/generate";
}
