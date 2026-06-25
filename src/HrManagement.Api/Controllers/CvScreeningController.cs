using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.CvScreening;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/ai/cv-screening")]
public sealed class CvScreeningController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CvScreeningResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CvScreeningResult>> ScreenCv(
        [FromBody] ScreenCvRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CvText) || string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest("Both cvText and jobDescription are required.");
        }

        var result = await sender.Send(new ScreenCvCommand(request.CvText, request.JobDescription), cancellationToken);
        return Ok(result);
    }
}

public sealed record ScreenCvRequest(string CvText, string JobDescription);
