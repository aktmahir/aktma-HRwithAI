using HrManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrManagement.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(department => department.Id);
        builder.Property(department => department.Name).HasMaxLength(120).IsRequired();
        builder.HasIndex(department => department.Name).IsUnique();
    }
}
