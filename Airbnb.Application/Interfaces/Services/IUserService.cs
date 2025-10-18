using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Authorization;

namespace Airbnb.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    Task UpdateUserAsync(UpdateDto dto, string userId);
}