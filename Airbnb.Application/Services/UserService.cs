using Airbnb.Application.CreatedEvent;
using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Mapster;

namespace Airbnb.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly IEventSender _eventSender;

    public UserService(IUserRepository userRepository, IUserManagerWrapper userManagerWrapper, IEventSender eventSender)
    {
        _userRepository = userRepository;
        _userManagerWrapper = userManagerWrapper;
        _eventSender = eventSender;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        var userProfile = await _userRepository.GetUserProfileAsync(userId);
        return userProfile;
    }
    
    public async Task UpdateUserAsync(UpdateDto dto, string userId)
    {
        var user = await _userManagerWrapper.FindByIdAsync(userId);
        if (user == null)
            return;
        
        dto.Adapt(user);
        await _userRepository.UpdateUserAsync(user);
        
        var updatedUser = user.Adapt<UserUpdatedEvent>();  
        var key = user.Id.ToString();
        
        await _eventSender.SendEvent(key, updatedUser);
    }
}