using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.Common.Exceptions;
using MediatR;

namespace HrManagement.Application.CvScreening;

public sealed class ScreenCvCommandHandler(ILlmService llmService)
    : IRequestHandler<ScreenCvCommand, CvScreeningResult>
{
    public Task<CvScreeningResult> Handle(ScreenCvCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CvText))
        {
            throw new ValidationException("CV text is required.");
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            throw new ValidationException("Job description is required.");
        }

        return llmService.ScreenCvAsync(request.CvText, request.JobDescription, cancellationToken);
    }
}
