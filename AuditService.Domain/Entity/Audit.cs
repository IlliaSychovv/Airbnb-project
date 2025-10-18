namespace AuditService.Domain.Entity;

public class Audit
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string ProfileJson { get; set; }
    public DateTime CreatedAt { get; set; }
}