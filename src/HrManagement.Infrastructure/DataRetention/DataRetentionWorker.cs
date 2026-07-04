using HrManagement.Domain.Leave;
using HrManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HrManagement.Infrastructure.DataRetention;

public sealed class DataRetentionWorker(
    HrDbContext dbContext,
    IOptions<DataRetentionOptions> options,
    ILogger<DataRetentionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "DataRetentionWorker started. CompletedLeaveRequestsRetentionDays={CompletedLeaveRequestsRetentionDays}, EnableArchiving={EnableArchiving}, EnablePurging={EnablePurging}",
            options.Value.CompletedLeaveRequestsRetentionDays,
            options.Value.EnableAutomaticArchive,
            options.Value.EnableAutomaticPurge);

        await RunAsync(stoppingToken);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var opts = options.Value;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-opts.CompletedLeaveRequestsRetentionDays);

        var completedLeaveRequests = dbContext.LeaveRequests
            .Where(lr => lr.Status != LeaveRequestStatus.Pending && lr.CreatedAt < cutoff)
            .ToList();

        logger.LogInformation("Found {Count} completed leave requests older than cutoff", completedLeaveRequests.Count);

        foreach (var leaveRequest in completedLeaveRequests)
        {
            if (opts.EnableAutomaticPurge)
            {
                dbContext.LeaveRequests.Remove(leaveRequest);
                logger.LogInformation("Purged leave request {LeaveRequestId} for employee {EmployeeId}", leaveRequest.Id, leaveRequest.EmployeeId);
            }
            else if (opts.EnableAutomaticArchive)
            {
                var archiveEntry = new ArchivedLeaveRequest(
                    originalId: leaveRequest.Id,
                    employeeId: leaveRequest.EmployeeId,
                    startDate: leaveRequest.StartDate,
                    endDate: leaveRequest.EndDate,
                    reason: leaveRequest.Reason,
                    status: leaveRequest.Status,
                    reviewedByEmployeeId: leaveRequest.ReviewedByEmployeeId,
                    reviewedAt: leaveRequest.ReviewedAt,
                    reviewNotes: leaveRequest.ReviewNotes,
                    createdAt: leaveRequest.CreatedAt,
                    archivedAt: DateTimeOffset.UtcNow);

                dbContext.Set<ArchivedLeaveRequest>().Add(archiveEntry);
                dbContext.LeaveRequests.Remove(leaveRequest);
                logger.LogInformation("Archived leave request {LeaveRequestId} for employee {EmployeeId}", leaveRequest.Id, leaveRequest.EmployeeId);
            }
        }

        if (completedLeaveRequests.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "DataRetentionWorker completed. ProcessedLeaveRequests={Count}",
                completedLeaveRequests.Count);
        }
        else
        {
            logger.LogInformation("DataRetentionWorker completed. Nothing to archive or purge.");
        }
    }
}
