using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces.Repositories;
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

    public async Task<UserLoginsDto?> GetUserLoginsAsync(Guid userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserLoginsDto
            {
                Name = u.Name,
                Email = u.Email
            })
            .FirstOrDefaultAsync();
    }
}