using Microsoft.AspNetCore.Identity;

namespace Airbnb.DTOs.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterDto dto);
}