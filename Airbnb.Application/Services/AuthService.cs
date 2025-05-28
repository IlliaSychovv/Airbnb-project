using Airbnb.DTOs.Interfaces;
using Airbnb.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.DTOs.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.Email
        };
        
        return await _userRepository.CreateAsync(user, dto.Password);
    }
}