using HrManagement.Domain.Leave;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrManagement.Infrastructure.Persistence.Configurations;

public sealed class ArchivedLeaveRequestConfiguration : IEntityTypeConfiguration<ArchivedLeaveRequest>
{
    public void Configure(EntityTypeBuilder<ArchivedLeaveRequest> builder)
    {
        builder.ToTable("archived_leave_requests");

        builder.HasKey(archived => archived.Id);

        builder.Property(archived => archived.OriginalId)
            .IsRequired();

        builder.Property(archived => archived.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(archived => archived.ReviewNotes)
            .HasMaxLength(500);

        builder.Property(archived => archived.Status)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(archived => archived.OriginalId)
            .IsUnique();

        builder.HasIndex(archived => archived.EmployeeId);

        builder.HasIndex(archived => archived.ArchivedAt);
    }
}
