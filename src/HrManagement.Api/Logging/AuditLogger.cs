using Microsoft.Extensions.Logging;

namespace HrManagement.Api.Logging;

public sealed class AuditLogger(ILogger<AuditLogger> logger)
{
    public void LogChange(string action, string resource, string? user, string details)
    {
        var entry = new AuditLogEntry(action, resource, user, details, DateTimeOffset.UtcNow);
        logger.LogInformation("Audit {Action} {Resource} by {User} :: {Details} at {Timestamp}",
            entry.Action,
            entry.Resource,
            entry.User ?? "anonymous",
            entry.Details,
            entry.Timestamp);
    }
}
