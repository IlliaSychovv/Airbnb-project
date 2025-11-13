using Airbnb.Application.DTO.Authorization;

namespace Airbnb.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterUserAsync(RegisterDto dto);
    Task<string?> LoginAsync(string username, string password);
}