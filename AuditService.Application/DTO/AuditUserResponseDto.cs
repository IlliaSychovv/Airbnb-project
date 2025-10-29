namespace AuditService.Application.DTO;

public record AuditUserResponseDto
{
    public int Port { get; set; }
    public UserLoginsDto? User { get; set; }
    public List<AuditUserDto>? Changes { get; set; } 
}