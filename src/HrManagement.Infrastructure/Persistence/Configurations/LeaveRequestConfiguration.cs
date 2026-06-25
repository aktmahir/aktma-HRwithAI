using HrManagement.Domain.Leave;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrManagement.Infrastructure.Persistence.Configurations;

public sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("leave_requests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Reason).HasMaxLength(500).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(request => request.ReviewNotes).HasMaxLength(500);
    }
}
