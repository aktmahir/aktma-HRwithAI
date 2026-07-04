using HrManagement.Application.Abstractions.Pagination;
using HrManagement.Application.Abstractions.Persistence;
using HrManagement.Domain.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Policy = "HrAdmin")]
public sealed class AuditLogsController(IAuditLogRepository auditLogRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLog>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<AuditLog>>> Search(
        [FromQuery] string? action = null,
        [FromQuery] string? resource = null,
        [FromQuery] string? user = null,
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var items = await auditLogRepository.SearchAsync(action, resource, user, from, to, page, pageSize, cancellationToken);
        var totalCount = await auditLogRepository.CountAsync(action, resource, user, from, to, cancellationToken);
        var result = PagedResult<AuditLog>.Create(items, totalCount, page, pageSize);

        return Ok(result);
    }
}
