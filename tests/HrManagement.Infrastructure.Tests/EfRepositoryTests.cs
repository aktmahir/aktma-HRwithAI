using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using HrManagement.Domain.Employees;

namespace HrManagement.Infrastructure.Tests;

public class EfRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<HrManagement.Infrastructure.Persistence.HrDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        await using var context = new HrManagement.Infrastructure.Persistence.HrDbContext(options);
        var employee = new HrManagement.Domain.Employees.Employee("John", "Doe", "john.doe@example.com", Guid.NewGuid(), Guid.NewGuid());
        await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();
        
        var repository = new EfRepository<Employee>(context);
        
        // Act
        var result = await repository.GetByIdAsync(employee.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Id, result!.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }
    
    [Fact]
    public async Task ListAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<HrManagement.Infrastructure.Persistence.HrDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        await using var context = new HrManagement.Infrastructure.Persistence.HrDbContext(options);
        var employee1 = new HrManagement.Domain.Employees.Employee("John", "Doe", "john.doe@example.com", Guid.NewGuid(), Guid.NewGuid());
        var employee2 = new HrManagement.Domain.Employees.Employee("Jane", "Smith", "jane.smith@example.com", Guid.NewGuid(), Guid.NewGuid());
        await context.Employees.AddRangeAsync(employee1, employee2);
        await context.SaveChangesAsync();
        
        var repository = new EfRepository<Employee>(context);
        
        // Act
        var result = await repository.ListAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
        foreach (var employee in result)
        {
            Assert.NotNull(employee);
        }
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<HrManagement.Infrastructure.Persistence.HrDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        await using var context = new HrManagement.Infrastructure.Persistence.HrDbContext(options);
        var repository = new EfRepository<Employee>(context);
        var employee = new HrManagement.Domain.Employees.Employee("New", "Employee", "new.employee@example.com", Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        await repository.AddAsync(employee);
        await context.SaveChangesAsync(); // Make sure to save after adding
        
        // Assert
        var result = await context.Employees.ToListAsync();
        Assert.Contains(employee, result);
    }
}