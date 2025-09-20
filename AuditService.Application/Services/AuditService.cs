using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace AuditService.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly IMonolithClient _monolithClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IAuditRepository auditRepository, IMonolithClient monolithClient, IHttpContextAccessor httpContextAccessor)
    {
        _auditRepository = auditRepository;
        _monolithClient = monolithClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuditResponseDto> GetAuditChangesAsync(Guid userId, DateTime since)
    {
        var list = await _auditRepository.GetUserChanges(userId, since);
        var auditDto = list.Adapt<List<AuditDto>>();

        var userLogins = await _monolithClient.GetUserLoginAsync(userId);
        var monolithPort = _httpContextAccessor.HttpContext.Connection.LocalPort;

        return new AuditResponseDto
        {
            Port = monolithPort,
            User = userLogins,
            Changes = auditDto
        };
    }
}