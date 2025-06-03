using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}