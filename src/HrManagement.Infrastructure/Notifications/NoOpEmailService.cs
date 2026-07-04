using HrManagement.Application.Abstractions.Notifications;

namespace HrManagement.Infrastructure.Notifications;

public sealed class NoOpEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
