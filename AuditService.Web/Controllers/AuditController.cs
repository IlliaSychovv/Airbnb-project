using AuditService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditService.Web.Controllers;

[ApiController]
[Route("api/v1/audit")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet("audit")]
    public async Task<IActionResult> GetAuditChanges([FromQuery] Guid userId, [FromQuery] DateTime at)
    {
        var list = await _auditService.GetAuditChangesAsync(userId, at);
        return Ok(list);
    }
}