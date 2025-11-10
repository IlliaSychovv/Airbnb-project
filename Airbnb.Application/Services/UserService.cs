using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Interfaces.Wrappers;
using Mapster;

namespace Airbnb.Application.Services;

public class UserService : IUserService
{
    private readonly IPaymentClient _paymentClient;
    private readonly IUserRepository _userRepository;
    private readonly IUserManagerWrapper _userManagerWrapper;

    public UserService(IUserRepository userRepository, IUserManagerWrapper userManagerWrapper,
        IPaymentClient paymentClient)
    {
        _userRepository = userRepository;
        _userManagerWrapper = userManagerWrapper;
        _paymentClient = paymentClient;
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
    }

    public async Task<BalanceResponse> GetUserBalanceAsync(string userId)
    {
        var user = await _userManagerWrapper.FindByIdAsync(userId);
        var balance = await _paymentClient.GetUserBalanceAsync(user.Id);
        return balance.Adapt<BalanceResponse>();
    }
}