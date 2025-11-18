using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Infrastructure.Data;
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

    public async Task<PagedResult<Apartment>> GetAllApartmentsAsync(int pageNumber, int pageSize, string? location = null)
    {
        var query = _context.Apartments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(a => a.Location.Contains(location));
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(a => a.Price)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Apartment>
        {
            Items = items,
            TotalCount = totalCount
        };
    }
    
    public async Task<PagedResult<Apartment>> GetAvailableApartmentsAsync(DateRange range, int pageNumber, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = _context.Apartments
            .AsNoTracking()
            .Include(a => a.Bookings)
            .Where(apartment => !apartment.Bookings
                .Any(b => b.ApartmentId == apartment.Id &&
                          b.BookingDate <= range.End &&
                          range.Start <= b.EndBookingDate));

        if (minPrice.HasValue)
            query = query.Where(a => a.Price >= minPrice.Value);
        
        if (maxPrice.HasValue)
            query = query.Where(a => a.Price <= maxPrice.Value);
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(a => a.Price)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Apartment>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<Apartment>> GetAllApartmentsWithBookingsAsync(int pageNumber, int pageSize)
    {
        var query =  _context.Apartments
            .AsNoTracking()
            .Include(a => a.Bookings)
            .Where(a => a.Bookings.Any());
        
        var totalCount = await query.CountAsync();
        
         var item = await query
            .OrderByDescending(a => a.Price)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

         return new PagedResult<Apartment>
         {
             Items = item,
             TotalCount = totalCount
         };
    }

    public async Task AddAsync(Apartment apartment)
    {
        await _context.Apartments.AddAsync(apartment);
        await _context.SaveChangesAsync();
    }
}