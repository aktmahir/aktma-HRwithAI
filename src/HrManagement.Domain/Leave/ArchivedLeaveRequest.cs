using HrManagement.Domain.Common;

namespace HrManagement.Domain.Leave;

public sealed class ArchivedLeaveRequest : Entity
{
    private ArchivedLeaveRequest()
    {
    }

    public ArchivedLeaveRequest(
        Guid originalId,
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        string reason,
        LeaveRequestStatus status,
        Guid? reviewedByEmployeeId,
        DateTimeOffset? reviewedAt,
        string? reviewNotes,
        DateTimeOffset createdAt,
        DateTimeOffset archivedAt)
    {
        OriginalId = originalId;
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;
        Status = status;
        ReviewedByEmployeeId = reviewedByEmployeeId;
        ReviewedAt = reviewedAt;
        ReviewNotes = reviewNotes;
        CreatedAt = createdAt;
        ArchivedAt = archivedAt;
    }

    public Guid OriginalId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public LeaveRequestStatus Status { get; private set; }
    public Guid? ReviewedByEmployeeId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }
    public DateTimeOffset ArchivedAt { get; private set; }
}
