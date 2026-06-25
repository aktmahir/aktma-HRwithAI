using HrManagement.Domain.Common;

namespace HrManagement.Domain.Leave;

public sealed class LeaveRequest : Entity
{
    private LeaveRequest()
    {
    }

    public LeaveRequest(Guid employeeId, DateOnly startDate, DateOnly endDate, string reason)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException("Employee id is required.", nameof(employeeId));
        }

        if (endDate < startDate)
        {
            throw new ArgumentException("Leave end date cannot be before the start date.", nameof(endDate));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Leave reason is required.", nameof(reason));
        }

        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason.Trim();
        Status = LeaveRequestStatus.Pending;
    }

    public Guid EmployeeId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public LeaveRequestStatus Status { get; private set; }
    public Guid? ReviewedByEmployeeId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }

    public void Approve(Guid reviewerEmployeeId, string? notes = null)
    {
        Review(reviewerEmployeeId, LeaveRequestStatus.Approved, notes);
    }

    public void Reject(Guid reviewerEmployeeId, string? notes = null)
    {
        Review(reviewerEmployeeId, LeaveRequestStatus.Rejected, notes);
    }

    private void Review(Guid reviewerEmployeeId, LeaveRequestStatus status, string? notes)
    {
        if (Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending leave requests can be reviewed.");
        }

        if (reviewerEmployeeId == Guid.Empty)
        {
            throw new ArgumentException("Reviewer employee id is required.", nameof(reviewerEmployeeId));
        }

        Status = status;
        ReviewedByEmployeeId = reviewerEmployeeId;
        ReviewedAt = DateTimeOffset.UtcNow;
        ReviewNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        MarkUpdated();
    }
}
