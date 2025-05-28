using Airbnb.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.DTOs.Interfaces;

public interface IUserRepository
{
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindByEmailAsync(string email);
}