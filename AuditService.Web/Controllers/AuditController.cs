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

    [HttpGet]
    public async Task<IActionResult> GetAuditChanges([FromQuery] Guid userId)
    {
        var list = await _auditService.GetAuditChangesAsync(userId);
        return Ok(list);
    }
}