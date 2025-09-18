using AuditService.Application.DTO;

namespace AuditService.Application.Interfaces;

public interface IAuditService
{
    Task<IEnumerable<AuditDto>> GetAuditChangesAsync(Guid userId, DateTime since);
}