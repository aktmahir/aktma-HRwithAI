using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize(Policy = "HrAdmin")]
public sealed class DepartmentsController(
    IRepository<Department> departments,
    IUnitOfWork unitOfWork,
    AuditLogger auditLogger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Department>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<Department>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var result = await departments.ListPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Department), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Department>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        return department is null ? NotFound() : Ok(department);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Department), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Department>> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = new Department(request.Name);
        await departments.AddAsync(department, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Create", "Department", User.Identity?.Name, $"Created department {department.Name}");
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        department.Rename(request.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Update", "Department", User.Identity?.Name, $"Updated department {department.Id}");
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        departments.Remove(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditLogger.LogChangeAsync("Delete", "Department", User.Identity?.Name, $"Deleted department {department.Id}");
        return NoContent();
    }
}

public sealed record CreateDepartmentRequest([System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(2)] string Name);
