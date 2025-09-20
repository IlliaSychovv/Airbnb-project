using AuditService.Application.DTO;

namespace AuditService.Application.Interfaces;

public interface IAuditService
{
    Task<AuditResponseDto> GetAuditChangesAsync(Guid userId, DateTime since);
}