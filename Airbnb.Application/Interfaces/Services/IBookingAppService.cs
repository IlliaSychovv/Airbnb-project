using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Services;

public interface IBookingAppService
{
    Task<List<Booking>> GetUserBookingsAsync(Guid userId);
    Task<Guid> CreateBookingAsync(Guid userId, Guid apartmentId, DateRange range);
}