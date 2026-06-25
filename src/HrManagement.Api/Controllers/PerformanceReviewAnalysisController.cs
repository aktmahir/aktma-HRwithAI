using HrManagement.Application.Abstractions.Ai;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/ai/performance-review-analysis")]
public sealed class PerformanceReviewAnalysisController(ILlmService llmService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PerformanceReviewAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PerformanceReviewAnalysisResult>> Analyze(
        [FromBody] AnalyzePerformanceReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewText))
        {
            return BadRequest("reviewText is required.");
        }

        var result = await llmService.AnalyzePerformanceReviewAsync(request.ReviewText, cancellationToken);
        return Ok(result);
    }
}

public sealed record AnalyzePerformanceReviewRequest(string ReviewText);
