using HrManagement.Domain.Employees;
using Xunit;

namespace HrManagement.Domain.Tests;

public sealed class EmployeeTests
{
    [Fact]
    public void CreateEmployee_WithEmptyRoleId_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Employee("Ada", "Lovelace", "ada@example.com", Guid.NewGuid(), Guid.Empty));

        Assert.Contains("Role id is required", exception.Message);
    }

    [Fact]
    public void AssignRole_WithValidId_UpdatesRoleId()
    {
        var employee = new Employee("Grace", "Hopper", "grace@example.com", Guid.NewGuid(), Guid.NewGuid());

        employee.AssignRole(Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, employee.RoleId);
    }
}
