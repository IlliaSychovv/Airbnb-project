using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBooking(Guid userId, Guid apartmentId, DateRange range);
}