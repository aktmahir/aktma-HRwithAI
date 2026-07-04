using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Persistence;

public sealed class RefreshTokenRepository(HrDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed && rt.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
