using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController(
    IRepository<Department> departments,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Department>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await departments.ListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Department>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        return department is null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = new Department(request.Name);
        await departments.AddAsync(department, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        department.Rename(request.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        departments.Remove(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record CreateDepartmentRequest(string Name);
