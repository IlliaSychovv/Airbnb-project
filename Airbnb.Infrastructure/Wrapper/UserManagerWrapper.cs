using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Infrastructure.Wrapper;

public class UserManagerWrapper : IUserManagerWrapper
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagerWrapper(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }
    
    public Task<ApplicationUser?> FindByNameAsync(string username)
    {
        return _userManager.FindByNameAsync(username);
    }

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }

    public Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return _userManager.GetRolesAsync(user);
    }

    public async Task AddToRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.AddToRoleAsync(user, role);
    }
}