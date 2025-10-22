using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.Metrics;

namespace AuditService.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly IMonolithClient _monolithClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly Meter _meter = new("AuditService.Metrics", "1.0");
    private static readonly Counter<long> _auditRequestedCounter =
        _meter.CreateCounter<long>("business_audit_requested_total", description: "Total number of times audits were requested");

    public AuditService(IAuditRepository auditRepository, IMonolithClient monolithClient, 
        IHttpContextAccessor httpContextAccessor)
    {
        _auditRepository = auditRepository;
        _monolithClient = monolithClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuditResponseDto> GetAuditChangesAsync(Guid userId)
    {
        _auditRequestedCounter.Add(1);
        
        var list = await _auditRepository.GetUserChanges(userId);
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