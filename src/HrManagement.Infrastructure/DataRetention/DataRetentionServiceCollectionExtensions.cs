using HrManagement.Infrastructure.DataRetention;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HrManagement.Infrastructure.DataRetention;

public static class DataRetentionServiceCollectionExtensions
{
    public static IServiceCollection AddDataRetention(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataRetentionOptions>(configuration.GetSection(DataRetentionOptions.SectionName));
        services.AddHostedService<DataRetentionWorker>();
        return services;
    }
}
