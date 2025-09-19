using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Airbnb.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IRedisService _redis;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IRedisService redis)
    {
        _userRepository = userRepository;
        _logger = logger;
        _redis = redis;
    }

    public async Task<UserLoginsDto?> GetUserLoginsAsync(Guid userId)
    {
        string cashedKey = $"user_{userId}";
        
        var cashedUser = await _redis.GetAsync<UserLoginsDto>(cashedKey);
        if (cashedUser != null)
        {
            _logger.LogInformation("Cache hit for user {UserId}", userId);
            return cashedUser;
        }
        
        _logger.LogInformation("Cache miss for user {UserId}", userId);
        
        var userLogins = await _userRepository.GetUserLoginsAsync(userId);
        await _redis.SetAsync(cashedKey, userLogins, TimeSpan.FromMinutes(5));
        
        return userLogins.Adapt<UserLoginsDto>();
    }
}