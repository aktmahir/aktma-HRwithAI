using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.CvScreening;
using HrManagement.Infrastructure.Metrics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/ai/cv-screening")]
[Authorize]
public sealed class CvScreeningController(ISender sender) : ControllerBase
{
    private const int MaxCvTextLength = 100_000;
    private const int MaxJobDescriptionLength = 50_000;

    [HttpPost]
    [RequestSizeLimit(1 * 1024 * 1024)]
    [ProducesResponseType(typeof(CvScreeningResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CvScreeningResult>> ScreenCv(
        [FromBody] ScreenCvRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CvText))
        {
            return BadRequest("CV text is required.");
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest("Job description is required.");
        }

        if (request.CvText.Length > MaxCvTextLength)
        {
            return BadRequest($"CV text exceeds maximum length of {MaxCvTextLength} characters.");
        }

        if (request.JobDescription.Length > MaxJobDescriptionLength)
        {
            return BadRequest($"Job description exceeds maximum length of {MaxJobDescriptionLength} characters.");
        }

        try
        {
            var result = await sender.Send(new ScreenCvCommand(request.CvText, request.JobDescription), cancellationToken);
            BusinessMetrics.AiCvScreeningCompleted.WithLabels("success").Inc();
            return Ok(result);
        }
        catch
        {
            BusinessMetrics.AiCvScreeningCompleted.WithLabels("failure").Inc();
            throw;
        }
    }
}

public sealed record ScreenCvRequest(string CvText, string JobDescription);
