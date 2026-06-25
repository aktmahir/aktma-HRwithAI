using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.PerformanceReviews;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/ai/performance-review-analysis")]
public sealed class PerformanceReviewAnalysisController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PerformanceReviewAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PerformanceReviewAnalysisResult>> Analyze(
        [FromBody] AnalyzePerformanceReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AnalyzePerformanceReviewCommand(request.ReviewText), cancellationToken);
        return Ok(result);
    }
}

public sealed record AnalyzePerformanceReviewRequest(string ReviewText);
