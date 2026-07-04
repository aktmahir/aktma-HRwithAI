using HrManagement.Domain.Audit;

namespace HrManagement.Application.Abstractions.Persistence;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> SearchAsync(string? action = null, string? resource = null, string? user = null, DateTimeOffset? from = null, DateTimeOffset? to = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? action = null, string? resource = null, string? user = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default);
}
