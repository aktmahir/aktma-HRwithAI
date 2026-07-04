using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HrManagement.Infrastructure;

public sealed class DataRetentionWorker(ILogger<DataRetentionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DataRetentionWorker started. This is a scaffold - implement actual retention logic here.");
        await Task.CompletedTask;
    }
}
