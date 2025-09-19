using Airbnb.Application.DTO;

namespace Airbnb.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserLoginsDto?> GetUserLoginsAsync(Guid userId);
}