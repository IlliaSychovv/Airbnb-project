using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Interfaces.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterDto dto);
    Task<string?> LoginAsync(string username, string password);
    Task UpdateUser(UpdateDto dto, string userId);
}