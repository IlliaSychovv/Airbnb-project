using Airbnb.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterDto dto);
    Task<string?> LoginAsync(string username, string password);
}