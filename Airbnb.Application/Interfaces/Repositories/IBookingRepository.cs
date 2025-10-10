using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<List<Booking>> GetByApartmentIdAndDateRange(Guid apartmentId, DateRange range);
    Task<Booking> GetByIdAsync(Guid bookingId);
    Task<bool> ExistsConflictAsync(Guid apartmentId, DateRange range);
    Task UpdateAsync(Booking booking);
    Task AddAsync(Booking booking);
}