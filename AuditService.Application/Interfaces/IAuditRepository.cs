using AuditService.Domain.Entity;

namespace AuditService.Application.Interfaces;

public interface IAuditRepository
{
    Task<List<Audit>> GetUserChanges(Guid userId, DateTime since);
}