using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.PerformanceReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/ai/performance-review-analysis")]
[Authorize]
public sealed class PerformanceReviewAnalysisController(ISender sender) : ControllerBase
{
    private const int MaxReviewTextLength = 50_000;

    [HttpPost]
    [ProducesResponseType(typeof(PerformanceReviewAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PerformanceReviewAnalysisResult>> Analyze(
        [FromBody] AnalyzePerformanceReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewText))
        {
            return BadRequest("Review text is required.");
        }

        if (request.ReviewText.Length > MaxReviewTextLength)
        {
            return BadRequest($"Review text exceeds maximum length of {MaxReviewTextLength} characters.");
        }

        var result = await sender.Send(new AnalyzePerformanceReviewCommand(request.ReviewText), cancellationToken);
        return Ok(result);
    }
}

public sealed record AnalyzePerformanceReviewRequest(string ReviewText);
