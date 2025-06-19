using Airbnb.Application.Interfaces;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class ApartmentRepository : IApartmentRepository
{
    private readonly AppDbContext _context;

    public ApartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Apartment>> GetAsync(int pageNumber, int pageSize, string? location = null)
    {
        var query = _context.Apartments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(a => a.Location.Contains(location));
        
        return await query
            .OrderByDescending(a => a.Id)
            .Skip(pageSize * pageNumber)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Apartment> GetByIdAsync(Guid apartmentId)
    {
        return await _context.Apartments.FindAsync(apartmentId);
    }
    
    public async Task<List<Apartment>> GetAvailableApartmentsAsync(DateRange range)
    {
        return await _context.Apartments
            .Include(a => a.Bookings)
            .Where(apartment => !apartment.Bookings
                .Any(b => b.ApartmentId == apartment.Id &&
                          b.BookingDate <= range.End &&
                          range.Start <= b.EndBookingDate))
            .ToListAsync();
    }
}