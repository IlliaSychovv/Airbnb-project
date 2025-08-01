namespace Airbnb.Application.DTOs.Authorization;

public record LoginDto
{
    public string Email { get; set; } = default;
    public string Password { get; set; } = default;
}