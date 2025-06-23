using Airbnb.Domain.Interfaces;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByApartmentIdAndDateRange(Guid apartmentId, DateRange range)
    {
        return await _context.Bookings
            .Where(u => u.ApartmentId == apartmentId &&
                                u.BookingDate <= range.End &&
                                range.Start <= u.EndBookingDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsConflictAsync(Guid apartmentId, DateRange range)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.ApartmentId == apartmentId &&
            b.BookingDate <= range.End &&
            range.Start <= b.EndBookingDate);
    }

    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }
}