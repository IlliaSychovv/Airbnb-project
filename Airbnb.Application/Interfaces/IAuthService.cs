using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.DTOs;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterDto dto);
    Task<string?> LoginAsync(string username, string password);
}