using HrManagement.Application.Abstractions.Notifications;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HrManagement.Infrastructure.Notifications;

public sealed class SmtpEmailService(IOptions<SmtpEmailOptions> options) : IEmailService
{
    private readonly SmtpClient _client = new()
    {
        Host = options.Value.Host,
        Port = options.Value.Port,
        EnableSsl = options.Value.EnableSsl,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(options.Value.Username, options.Value.Password),
        DeliveryMethod = SmtpDeliveryMethod.Network
    };

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new MailMessage(options.Value.FromAddress, to, subject, body)
        {
            IsBodyHtml = true
        };

        await _client.SendMailAsync(message, cancellationToken);
    }
}
