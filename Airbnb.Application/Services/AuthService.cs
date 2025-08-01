using System.Text.Json;
using Airbnb.Application.DTOs.Authorization;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Options;
using Microsoft.Extensions.Options;
using Airbnb.Application.CreatedEvent;
using Mapster;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly string _topic;
    
    public AuthService(IUserManagerWrapper userManagerWrapper, IJwtTokenService jwtTokenService, 
        IKafkaProducer producer, IOptions<KafkaOptions> options)
    {
        _userManagerWrapper = userManagerWrapper;
        _jwtTokenService = jwtTokenService;
        _kafkaProducer = producer;
        _topic = options.Value.Topic;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterDto dto)
    {
        var user = dto.Adapt<ApplicationUser>();
        user.UserName = dto.Email;
        user.ExternalId = Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow;
 
        var result = await _userManagerWrapper.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _userManagerWrapper.AddToRoleAsync(user, dto.Role);

            var userEvent = user.Adapt<UserCreatedEvent>();
            
            var jsonMessage = JsonSerializer.Serialize(userEvent);
            await _kafkaProducer.ProduceAsync(_topic, jsonMessage);
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
        var jsonMessage = JsonSerializer.Serialize(updatedUser);
        await _kafkaProducer.ProduceAsync(_topic, jsonMessage);
    }
}