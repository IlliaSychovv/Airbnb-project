namespace AuditService.Application.DTO;

public record AuditDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ProfileJson { get; set; }
    public DateTime CreatedAt { get; set; }
}