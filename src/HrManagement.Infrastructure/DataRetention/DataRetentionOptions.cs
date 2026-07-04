using Microsoft.Extensions.Options;

namespace HrManagement.Infrastructure.DataRetention;

public sealed class DataRetentionOptions
{
    public const string SectionName = "DataRetention";
    public int CompletedLeaveRequestsRetentionDays { get; init; } = 365;
    public int AuditLogRetentionDays { get; init; } = 90;
    public bool EnableAutomaticPurge { get; init; } = false;
    public bool EnableAutomaticArchive { get; init; } = true;
}
