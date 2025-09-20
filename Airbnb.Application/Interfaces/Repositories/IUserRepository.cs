using Airbnb.Application.DTO;
using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserProfileDto?> GetUserLoginsAsync(Guid userId);
    Task UpdateUserAsync(ApplicationUser user, string userId);
}