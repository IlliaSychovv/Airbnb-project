using AuditService.Application.DTO;

namespace AuditService.Application.Interfaces;

public interface IMonolithClient
{
    Task<UserLoginsDto?> GetUserLoginAsync(Guid userId);
}