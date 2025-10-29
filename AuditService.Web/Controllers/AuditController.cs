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

    [HttpGet("user")]
    public async Task<IActionResult> GetUserChanges([FromQuery] Guid userId)
    {
        var list = await _auditService.GetUserChangesAsync(userId);
        return Ok(list);
    }

    [HttpGet("apartment")]
    public async Task<IActionResult> GetApartmentChanges([FromQuery] Guid apartmentId)
    {
        var list = await _auditService.GetApartmentsAsync(apartmentId);
        return Ok(list);
    }
}