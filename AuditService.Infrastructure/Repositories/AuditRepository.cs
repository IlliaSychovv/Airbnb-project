using AuditService.Application.Interfaces;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public AuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Audit>> GetUserChanges(Guid userId, DateTime at)
    {
        var list = await _context.Audits
            .Where(a => a.UserId == userId && a.CreatedAt <= at)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return list;
    }
}