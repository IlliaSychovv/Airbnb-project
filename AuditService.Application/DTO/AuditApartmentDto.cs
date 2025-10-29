namespace AuditService.Application.DTO;

public record AuditApartmentDto
{
    public Guid Id { get; init; }
    public Guid ApartmentId { get; init; }
    public string ProfileJson { get; init; }
    public DateTime CreatedAt { get; init; }
}