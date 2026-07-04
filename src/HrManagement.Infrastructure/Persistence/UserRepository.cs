using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Persistence;

public sealed class UserRepository(HrDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant(), cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<User>().AddAsync(user, cancellationToken);
    }
}
