using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Services;

public interface IBookingService
{
    Task<Booking> CreateBooking(Guid userId, Guid apartmentId, DateRange range);
    Task MarkAsPaid(Guid bookingId);
    Task MarkAsCancelled(Guid bookingId);
    Task<List<Booking>> GetUserBookingsAsync(Guid userId);
}