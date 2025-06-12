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

    public async Task<List<Apartment>> GetAllAsync()
    {
        return await _context.Apartments.ToListAsync();
    }

    public async Task<Apartment> GetByIdAsync(Guid apartmentId)
    {
        return await _context.Apartments.FindAsync(apartmentId);
    }
    
    public async Task<List<Apartment>> GetAvailableApartmentsAsync(DateRange range)
    {
        return await _context.Apartments
            .Where(apartment => !_context.Bookings
                .Any(b => b.ApartmentId == apartment.Id &&
                          b.BookingDate <= range.End &&
                          range.Start <= b.EndBookingDate))
            .ToListAsync();
    }
}