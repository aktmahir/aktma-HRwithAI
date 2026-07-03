namespace HrManagement.Api.Logging;

public sealed record AuditLogEntry(
    string Action,
    string Resource,
    string? User,
    string Details,
    DateTimeOffset Timestamp);
