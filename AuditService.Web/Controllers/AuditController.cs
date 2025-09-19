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
    public async Task<IActionResult> GetAuditChanges([FromQuery] Guid userId, [FromQuery] DateTime at, [FromServices] IMonolithClient monolithClient)
    {
        var list = await _auditService.GetAuditChangesAsync(userId, at);
        var userLogins = await monolithClient.GetUserLoginAsync(userId);
        var localPort = HttpContext.Connection.LocalPort;
        return Ok(new
        {
            port = localPort,
            user = userLogins,
            lists = list,
        });
    }
}