using System.ComponentModel.DataAnnotations;
using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Pagination;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using HrManagement.Infrastructure.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "HrAdmin")]
public sealed class EmployeesController(
    IRepository<Employee> employees,
    IUnitOfWork unitOfWork,
    AuditLogger auditLogger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Employee>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<Employee>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var items = await employees.ListPagedAsync(page, pageSize, cancellationToken);
        var totalCount = await employees.CountAsync(cancellationToken);
        var result = PagedResult<Employee>.Create(items, totalCount, page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Employee>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Employee>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.DepartmentId,
            request.RoleId);

        await employees.AddAsync(employee, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Create", "Employee", User.Identity?.Name, $"Created employee {employee.Id}");
        BusinessMetrics.EmployeesCreated.Inc();
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.ChangeName(request.FirstName, request.LastName);
        employee.ChangeEmail(request.Email);
        employee.AssignDepartment(request.DepartmentId);
        employee.AssignRole(request.RoleId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Update", "Employee", User.Identity?.Name, $"Updated employee {employee.Id}");
        BusinessMetrics.EmployeesUpdated.Inc();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employees.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Delete", "Employee", User.Identity?.Name, $"Deleted employee {employee.Id}");
        BusinessMetrics.EmployeesDeleted.Inc();
        return NoContent();
    }
}

public sealed record CreateEmployeeRequest(
    [Required] [StringLength(80)] string FirstName,
    [Required] [StringLength(80)] string LastName,
    [Required] [EmailAddress] [StringLength(254)] string Email,
    [Required] Guid DepartmentId,
    [Required] Guid RoleId);

public sealed record UpdateEmployeeRequest(
    [Required] [StringLength(80)] string FirstName,
    [Required] [StringLength(80)] string LastName,
    [Required] [EmailAddress] [StringLength(254)] string Email,
    [Required] Guid DepartmentId,
    [Required] Guid RoleId);
