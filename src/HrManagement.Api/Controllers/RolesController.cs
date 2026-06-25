using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/roles")]
public sealed class RolesController(
    IRepository<Role> roles,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Role>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await roles.ListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Role>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var role = await roles.GetByIdAsync(id, cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = new Role(request.Title);
        await roles.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await roles.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return NotFound();
        }

        role.Rename(request.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var role = await roles.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return NotFound();
        }

        roles.Remove(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record CreateRoleRequest(string Title);
