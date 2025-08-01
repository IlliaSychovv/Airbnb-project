using AuditService.Application.DTOs;
using AuditService.Application.Interfaces;
using Mapster;

namespace AuditService.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;

    public AuditService(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<AuditDto>> GetAuditChangesAsync(Guid userId, DateTime at)
    {
        var list = await _auditRepository.GetUserChanges(userId, at);
        var auditDto = list.Adapt<List<AuditDto>>();
        return auditDto;
    }
}