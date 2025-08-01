using Airbnb.Domain.Entities;

namespace Airbnb.Application.DTOs.Authorization;

public record RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = RoleConstants.Client;
}