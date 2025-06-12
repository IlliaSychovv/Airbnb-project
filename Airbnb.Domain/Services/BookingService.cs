using Airbnb.Domain.DomainInterfaces;
using Airbnb.Domain.ValueObject;
using Airbnb.Domain.Entities;

namespace Airbnb.Domain.Services;

public class BookingService
{
    private readonly IApartmentAvailability _apartmentAvailability;
    private readonly IBookingConflict _bookingConflict;

    public BookingService(IApartmentAvailability apartmentAvailability, IBookingConflict bookingConflict)
    {
        _apartmentAvailability = apartmentAvailability;
        _bookingConflict = bookingConflict;
    }

    public async Task<Booking> CreateBooking(Guid userId, Guid apartmentId, DateRange range)
    {
        if (!await _apartmentAvailability.IsAvailableAsync(apartmentId, range))
            throw new InvalidOperationException("Apartment is not available");
        
        if (await _bookingConflict.HasConflictAsync(apartmentId, range))
            throw new InvalidOperationException("Booking conflict");
        
        return Booking.Create(userId, apartmentId, range);
    }
}