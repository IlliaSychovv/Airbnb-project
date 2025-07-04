using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}