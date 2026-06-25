using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HrManagement.Infrastructure.Persistence;

public sealed class DatabaseInitializer(
    IServiceScopeFactory scopeFactory,
    IOptions<DatabaseOptions> options)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!options.Value.ApplyMigrationsOnStartup)
        {
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HrDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
