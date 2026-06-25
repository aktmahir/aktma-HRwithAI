using HrManagement.Application.Abstractions.Ai;
using MediatR;

namespace HrManagement.Application.CvScreening;

public sealed class ScreenCvCommandHandler(ILlmService llmService)
    : IRequestHandler<ScreenCvCommand, CvScreeningResult>
{
    public Task<CvScreeningResult> Handle(ScreenCvCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CvText))
        {
            throw new ArgumentException("CV text is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            throw new ArgumentException("Job description is required.", nameof(request));
        }

        return llmService.ScreenCvAsync(request.CvText, request.JobDescription, cancellationToken);
    }
}
