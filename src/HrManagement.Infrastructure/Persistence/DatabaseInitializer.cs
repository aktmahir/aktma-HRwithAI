using BCrypt.Net;
using HrManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HrManagement.Infrastructure.Persistence;

public sealed class DatabaseInitializer(
    IServiceScopeFactory scopeFactory,
    IOptions<DatabaseOptions> options,
    IPasswordHasher passwordHasher)
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

        if (!dbContext.Users.Any(u => u.Username == "admin"))
        {
            var defaultPassword = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD") ?? "ChangeMe123!";
            var admin = new User("admin", passwordHasher.Hash(defaultPassword), "Admin");
            await dbContext.Users.AddAsync(admin, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
