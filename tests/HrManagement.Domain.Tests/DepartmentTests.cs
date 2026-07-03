using HrManagement.Domain.Employees;
using Xunit;

namespace HrManagement.Domain.Tests;

public sealed class DepartmentTests
{
    [Fact]
    public void Rename_WithValidName_UpdatesName()
    {
        var department = new Department("Engineering");

        department.Rename("Platform");

        Assert.Equal("Platform", department.Name);
    }

    [Fact]
    public void Rename_WithEmptyName_ThrowsArgumentException()
    {
        var department = new Department("Engineering");

        var ex = Assert.Throws<ArgumentException>(() => department.Rename("   "));

        Assert.Contains("Department name is required", ex.Message);
    }
}
