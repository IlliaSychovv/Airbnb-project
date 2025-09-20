using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Airbnb.Infrastructure.Repositories;

public class CachedUserRepository : IUserRepository
{
    private const string MonolithPrefix = "Monolith";
    private readonly IUserRepository _repo;
    private readonly IRedisService _redis;
    private readonly IUserManagerWrapper _userManager;
    private readonly ILogger<CachedUserRepository> _logger;

    public CachedUserRepository(IUserRepository repo, IRedisService redis, 
        IUserManagerWrapper userManager, ILogger<CachedUserRepository> logger)
    {
        _repo = repo;
        _redis = redis;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        string cashedKey = $"{MonolithPrefix}_user_{userId}";
        
        var cashedUser = await _redis.GetAsync<UserProfileDto>(cashedKey);
        if (cashedUser != null)
        {
            _logger.LogDebug("Cache hit for user {UserId}", userId);
            return cashedUser;
        }
        
        _logger.LogDebug("Cache miss for user {UserId}", userId);
        
        var user = await _repo.GetUserProfileAsync(userId);
        if (user != null)
            await _redis.SetAsync(cashedKey, user, TimeSpan.FromMinutes(5));
        
        return user;
    }

    public async Task UpdateUserAsync(ApplicationUser entity)
    {
        var user = await _userManager.FindByIdAsync(entity.Id.ToString());
        if (user == null)
            return;
        
        entity.Adapt(user);
        await _userManager.UpdateAsync(user);
        
        string cashedKey = $"{MonolithPrefix}_user_{entity.Id}";
        await _redis.DeleteDataAsync(cashedKey);
        
        _logger.LogDebug("Cache invalidated for user {UserId}", entity.Id);
    }
}