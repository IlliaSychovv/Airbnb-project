using Airbnb.Domain.ValueObject;
using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IBookingAppService
{
    Task<List<Booking>> GetUserBookingsAsync(Guid userId);
    Task<Guid> CreateBookingAsync(Guid userId, Guid apartmentId, DateRange range);
}