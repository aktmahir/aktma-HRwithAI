using HrManagement.Domain.Leave;

namespace HrManagement.Application.Abstractions.Notifications;

public sealed record LeaveEventNotification(
    Guid LeaveRequestId,
    Guid EmployeeId,
    string EmployeeName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason,
    LeaveRequestStatus Status,
    string? ReviewerNotes,
    DateTimeOffset ReviewedAt);

public sealed record AiCompletionNotification(
    string Feature,
    Guid CorrelationId,
    bool Success,
    string? ErrorMessage = null);
