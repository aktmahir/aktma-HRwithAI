using HrManagement.Domain.Common;

namespace HrManagement.Domain.Audit;

public sealed class AuditLog : Entity
{
    private AuditLog()
    {
    }

    public AuditLog(string action, string resource, string? user, string details, DateTimeOffset timestamp)
    {
        ChangeAction(action);
        ChangeResource(resource);
        ChangeUser(user);
        ChangeDetails(details);
        ChangeTimestamp(timestamp);
    }

    public string Action { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string? User { get; private set; }
    public string Details { get; private set; } = string.Empty;
    public DateTimeOffset Timestamp { get; private set; }

    public void ChangeAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action is required.", nameof(action));
        }

        Action = action.Trim();
        MarkUpdated();
    }

    public void ChangeResource(string resource)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Resource is required.", nameof(resource));
        }

        Resource = resource.Trim();
        MarkUpdated();
    }

    public void ChangeUser(string? user)
    {
        User = string.IsNullOrWhiteSpace(user) ? null : user.Trim();
    }

    public void ChangeDetails(string details)
    {
        if (string.IsNullOrWhiteSpace(details))
        {
            throw new ArgumentException("Details are required.", nameof(details));
        }

        Details = details.Trim();
        MarkUpdated();
    }

    public void ChangeTimestamp(DateTimeOffset timestamp)
    {
        Timestamp = timestamp;
    }
}
