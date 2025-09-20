namespace AuditService.Application.DTO;

public record AuditResponseDto
{
    public int Port { get; set; }
    public UserLoginsDto? User { get; set; }
    public List<AuditDto>? Changes { get; set; } 
}