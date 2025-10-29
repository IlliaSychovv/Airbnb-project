using AuditService.Application.Interfaces;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using MongoDB.Driver;

namespace AuditService.Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly MongoDbContext _context;

    public AuditRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Audit>> GetUserChanges(Guid userId)
    {
        var filter = Builders<Audit>.Filter.Eq(a => a.UserId, userId);
        var list = await _context.Audits
            .Find(filter)
            .ToListAsync();

        return list;
    }

    public async Task<List<Apartment>> GetApartmentChanges(Guid apartmentId)
    {
        var filter = Builders<Apartment>.Filter.Eq(a => a.ApartmentId, apartmentId);
        var list = await _context.Apartments
            .Find(filter)
            .ToListAsync();
        
        return list;
    }
}