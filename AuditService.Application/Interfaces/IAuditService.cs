using AuditService.Application.DTO;

namespace AuditService.Application.Interfaces;

public interface IAuditService
{
    Task<AuditUserResponseDto> GetUserChangesAsync(Guid userId);
    Task<List<AuditApartmentDto>> GetApartmentsAsync(Guid apartmentId);
}