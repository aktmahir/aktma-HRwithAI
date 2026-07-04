using HrManagement.Application.Abstractions.Ai;
using HrManagement.Application.Abstractions.Notifications;
using HrManagement.Application.Common.Exceptions;
using MediatR;

namespace HrManagement.Application.CvScreening;

public sealed class ScreenCvCommandHandler(ILlmService llmService, IEmailService emailService)
    : IRequestHandler<ScreenCvCommand, CvScreeningResult>
{
    public async Task<CvScreeningResult> Handle(ScreenCvCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CvText))
        {
            throw new ValidationException("CV text is required.");
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            throw new ValidationException("Job description is required.");
        }

        try
        {
            var result = await llmService.ScreenCvAsync(request.CvText, request.JobDescription, cancellationToken);

            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendAsync(
                        to: "hr@example.com",
                        subject: "CV Screening Completed",
                        body: $"CV screening completed. Match percentage: {result.MatchPercentage}%");
                }
                catch
                {
                }
            }, cancellationToken);

            return result;
        }
        catch
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendAsync(
                        to: "hr@example.com",
                        subject: "CV Screening Failed",
                        body: "CV screening failed due to an internal error.");
                }
                catch
                {
                }
            }, cancellationToken);

            throw;
        }
    }
}
