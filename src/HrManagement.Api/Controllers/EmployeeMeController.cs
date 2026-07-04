using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/employees/me")]
[Authorize]
public sealed class EmployeeMeController(
    IRepository<Employee> employees,
    IUserRepository users) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Employee>> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.EmployeeId.HasValue)
        {
            return NotFound("Employee profile not linked to user account.");
        }

        var employee = await employees.GetByIdAsync(user.EmployeeId.Value, cancellationToken);
        return employee is null ? NotFound("Employee record not found.") : Ok(employee);
    }
}
