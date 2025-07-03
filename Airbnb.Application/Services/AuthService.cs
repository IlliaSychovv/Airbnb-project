using Airbnb.Application.DTOs;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Mapster;
using Airbnb.Application.Interfaces;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly IJwtTokenService _jwtTokenService;
    
    public AuthService(IUserManagerWrapper userManagerWrapper, IJwtTokenService jwtTokenService)
    {
        _userManagerWrapper = userManagerWrapper;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterDto dto)
    {
        var user = dto.Adapt<ApplicationUser>();
        user.UserName = dto.Email;

        return await _userManagerWrapper.CreateAsync(user, dto.Password);
    }
    
    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await _userManagerWrapper.FindByNameAsync(username);
        if (user == null)
            return null;

        var passwordValid = await _userManagerWrapper.CheckPasswordAsync(user, password);
        if (!passwordValid)
            return null;

        var roles = await _userManagerWrapper.GetRolesAsync(user);

        return _jwtTokenService.GenerateToken(user, roles);
    }
}