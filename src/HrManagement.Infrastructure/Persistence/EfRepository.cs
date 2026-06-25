using HrManagement.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Persistence;

public sealed class EfRepository<TEntity>(HrDbContext dbContext) : IRepository<TEntity>
    where TEntity : class
{
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<TEntity>().FindAsync([id], cancellationToken).AsTask();
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    }

    public void Remove(TEntity entity)
    {
        dbContext.Set<TEntity>().Remove(entity);
    }
}
