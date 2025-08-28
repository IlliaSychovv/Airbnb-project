using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.CreatedEvent;
using Airbnb.Application.DTO.Authorization;
using Mapster;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEventSender _eventSender;
     
    public AuthService(IUserManagerWrapper userManagerWrapper, IJwtTokenService jwtTokenService, 
        IEventSender eventSender)
    {
        _userManagerWrapper = userManagerWrapper;
        _jwtTokenService = jwtTokenService;
        _eventSender = eventSender;
     }

    public async Task<IdentityResult> RegisterUserAsync(RegisterDto dto)
    {
        var user = dto.Adapt<ApplicationUser>();
        user.UserName = dto.Name;
        user.ExternalId = Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow;
 
        var result = await _userManagerWrapper.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _userManagerWrapper.AddToRoleAsync(user, dto.Role);

            var userEvent = user.Adapt<UserCreatedEvent>();
            var key = user.Id.ToString();
            
            await _eventSender.SendEvent(key, userEvent);
        }
        
        return result;
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

    public async Task UpdateUser(UpdateDto dto, string userId)
    {
        var user = await _userManagerWrapper.FindByIdAsync(userId);
        if (user == null)
            return;
        
        user.Email = dto.Email;
        user.Name = dto.Name;
        user.PhoneNumber = dto.PhoneNumber;
        
        await _userManagerWrapper.UpdateAsync(user);
        
        var updatedUser = user.Adapt<UserUpdatedEvent>();  
        var key = user.Id.ToString();
        
        await _eventSender.SendEvent(key, updatedUser);
    }
}