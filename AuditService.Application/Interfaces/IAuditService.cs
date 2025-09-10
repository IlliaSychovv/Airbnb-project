using AuditService.Application.DTOs;

namespace AuditService.Application.Interfaces;

public interface IAuditService
{
    Task<IEnumerable<AuditDto>> GetAuditChangesAsync(Guid userId, DateTime since);
}