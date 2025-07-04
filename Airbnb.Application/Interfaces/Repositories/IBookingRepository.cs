using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<List<Booking>> GetByApartmentIdAndDateRange(Guid apartmentId, DateRange range);
    Task<bool> ExistsConflictAsync(Guid apartmentId, DateRange range);
    Task AddAsync(Booking booking);
}