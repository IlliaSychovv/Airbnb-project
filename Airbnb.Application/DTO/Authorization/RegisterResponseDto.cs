namespace Airbnb.Application.DTO.Authorization;

public record RegisterResponseDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}