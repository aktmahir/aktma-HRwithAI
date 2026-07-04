using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace HrManagement.Api.Logging;

public sealed class AuditLogger(ILogger<AuditLogger> logger)
{
    private static readonly Regex EmailRegex = new(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public void LogChange(string action, string resource, string? user, string details)
    {
        var safeDetails = EmailRegex.Replace(details ?? string.Empty, "[REDACTED-EMAIL]");
        var entry = new AuditLogEntry(action, resource, user, safeDetails, DateTimeOffset.UtcNow);
        logger.LogInformation("Audit {Action} {Resource} by {User} :: {Details} at {Timestamp}",
            entry.Action,
            entry.Resource,
            entry.User ?? "anonymous",
            entry.Details,
            entry.Timestamp);
    }
}
