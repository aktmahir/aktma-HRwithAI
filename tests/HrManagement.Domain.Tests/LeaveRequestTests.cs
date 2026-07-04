using HrManagement.Domain.Leave;
using Xunit;

namespace HrManagement.Domain.Tests;

public sealed class LeaveRequestTests
{
    [Fact]
    public void Create_WithEmptyEmployeeId_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new LeaveRequest(Guid.Empty, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation"));

        Assert.Contains("Employee id is required", exception.Message);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 5), new DateOnly(2026, 1, 1), "Vacation"));

        Assert.Contains("Leave end date cannot be before the start date", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyReason_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), string.Empty));

        Assert.Contains("Leave reason is required", exception.Message);
    }

    [Fact]
    public void Create_WithWhiteSpaceReason_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "   "));

        Assert.Contains("Leave reason is required", exception.Message);
    }

    [Fact]
    public void Create_WithValidInput_SetsDefaults()
    {
        var employeeId = Guid.NewGuid();
        var leaveRequest = new LeaveRequest(employeeId, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        Assert.Equal(employeeId, leaveRequest.EmployeeId);
        Assert.Equal(new DateOnly(2026, 1, 1), leaveRequest.StartDate);
        Assert.Equal(new DateOnly(2026, 1, 5), leaveRequest.EndDate);
        Assert.Equal("Vacation", leaveRequest.Reason);
        Assert.Equal(LeaveRequestStatus.Pending, leaveRequest.Status);
    }

    [Fact]
    public void Approve_ChangesStatusToApproved()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");
        var reviewerId = Guid.NewGuid();

        leaveRequest.Approve(reviewerId, "OK");

        Assert.Equal(LeaveRequestStatus.Approved, leaveRequest.Status);
        Assert.Equal(reviewerId, leaveRequest.ReviewedByEmployeeId);
        Assert.Equal("OK", leaveRequest.ReviewNotes);
    }

    [Fact]
    public void Reject_ChangesStatusToRejected()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");
        var reviewerId = Guid.NewGuid();

        leaveRequest.Reject(reviewerId, "Denied");

        Assert.Equal(LeaveRequestStatus.Rejected, leaveRequest.Status);
        Assert.Equal(reviewerId, leaveRequest.ReviewedByEmployeeId);
        Assert.Equal("Denied", leaveRequest.ReviewNotes);
    }

    [Fact]
    public void Approve_OnNonPending_ThrowsInvalidOperationException()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");
        leaveRequest.Approve(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => leaveRequest.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void Reject_OnNonPending_ThrowsInvalidOperationException()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");
        leaveRequest.Reject(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => leaveRequest.Reject(Guid.NewGuid()));
    }

    [Fact]
    public void Approve_WithEmptyReviewerId_ThrowsArgumentException()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        Assert.Throws<ArgumentException>(() => leaveRequest.Approve(Guid.Empty));
    }

    [Fact]
    public void Reject_WithEmptyReviewerId_ThrowsArgumentException()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        Assert.Throws<ArgumentException>(() => leaveRequest.Reject(Guid.Empty));
    }

    [Fact]
    public void Approve_WithNullNotes_SetsReviewNotesToNull()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        leaveRequest.Approve(Guid.NewGuid(), null);

        Assert.Null(leaveRequest.ReviewNotes);
    }

    [Fact]
    public void Reject_WithWhiteSpaceNotes_SetsReviewNotesToNull()
    {
        var leaveRequest = new LeaveRequest(Guid.NewGuid(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 5), "Vacation");

        leaveRequest.Reject(Guid.NewGuid(), "   ");

        Assert.Null(leaveRequest.ReviewNotes);
    }
}
