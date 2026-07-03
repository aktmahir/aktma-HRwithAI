using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Policy = "HrAdmin")]
public sealed class RolesController(
    IRepository<Role> roles,
    IUnitOfWork unitOfWork,
    AuditLogger auditLogger) : ControllerBase
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

        auditLogger.LogChange("Create", "Role", User.Identity?.Name, $"Created role {role.Id}");
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

        auditLogger.LogChange("Update", "Role", User.Identity?.Name, $"Updated role {role.Id}");
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

        auditLogger.LogChange("Delete", "Role", User.Identity?.Name, $"Deleted role {role.Id}");
        return NoContent();
    }
}

public sealed record CreateRoleRequest([System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(2)] string Title);
