using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
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
    public async Task<ActionResult<IReadOnlyList<Employee>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var result = await employees.ListPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Employee>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
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

        auditLogger.LogChange("Create", "Employee", User.Identity?.Name, $"Created employee {employee.Id}");
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:guid}")]
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

        auditLogger.LogChange("Update", "Employee", User.Identity?.Name, $"Updated employee {employee.Id}");
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employees.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogger.LogChange("Delete", "Employee", User.Identity?.Name, $"Deleted employee {employee.Id}");
        return NoContent();
    }
}

public sealed record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    Guid RoleId);

public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    Guid RoleId);
