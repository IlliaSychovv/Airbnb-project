using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Interfaces;

public interface IUserManagerWrapper
{
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByNameAsync(string username);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task AddToRoleAsync(ApplicationUser user, string role);
}