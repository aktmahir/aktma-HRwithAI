using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Persistence;

public sealed class AuditLogRepository(HrDbContext dbContext) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<AuditLog>().AddAsync(auditLog, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> SearchAsync(string? action = null, string? resource = null, string? user = null, DateTimeOffset? from = null, DateTimeOffset? to = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var query = dbContext.Set<AuditLog>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(resource))
        {
            query = query.Where(a => a.Resource == resource);
        }

        if (!string.IsNullOrWhiteSpace(user))
        {
            query = query.Where(a => a.User == user);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? action = null, string? resource = null, string? user = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<AuditLog>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(resource))
        {
            query = query.Where(a => a.Resource == resource);
        }

        if (!string.IsNullOrWhiteSpace(user))
        {
            query = query.Where(a => a.User == user);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        return await query.CountAsync(cancellationToken);
    }
}
