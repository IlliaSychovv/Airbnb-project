using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.DTO.Authorization;
using Contracts.MonolithEvents;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEventSender _eventSender;
    private readonly ILogger<AuthService> _logger;
     
    public AuthService(IUserManagerWrapper userManagerWrapper, IJwtTokenService jwtTokenService, 
        IEventSender eventSender, ILogger<AuthService> logger)
    {
        _userManagerWrapper = userManagerWrapper;
        _jwtTokenService = jwtTokenService;
        _eventSender = eventSender;
        _logger = logger;
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

            await _eventSender.SaveToOutbox(userEvent, key);
            _logger.LogInformation("Send to Outbox event {@userEvent} for user {user.Id}", userEvent, user.Id);
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
}