using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserProfileDto
            {
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                CreatedAt = u.CreatedAt,
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateUserAsync(ApplicationUser user)
    { 
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}