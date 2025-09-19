using Airbnb.Application.DTO;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserLoginsDto?> GetUserLoginsAsync(Guid userId); 
}