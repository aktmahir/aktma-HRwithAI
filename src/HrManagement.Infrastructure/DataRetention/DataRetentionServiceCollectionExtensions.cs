using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HrManagement.Infrastructure;

public static class DataRetentionServiceCollectionExtensions
{
    public static IServiceCollection AddDataRetention(this IServiceCollection services)
    {
        services.AddHostedService<DataRetentionWorker>();
        return services;
    }
}
