using HrManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(employee => employee.Id);
        builder.Property(employee => employee.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(employee => employee.LastName).HasMaxLength(80).IsRequired();
        builder.Property(employee => employee.Email).HasMaxLength(254).IsRequired();
        builder.HasIndex(employee => employee.Email).IsUnique();

        builder
            .HasOne(employee => employee.Department)
            .WithMany()
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(employee => employee.Role)
            .WithMany()
            .HasForeignKey(employee => employee.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
