using Airbnb.Domain.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Domain.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }
    
    public async Task<Booking> CreateBooking(Guid userId, Guid apartmentId, DateRange range)
    {
        bool hasConflict = await _bookingRepository.ExistsConflictAsync(apartmentId, range);

        if (hasConflict)
            throw new InvalidOperationException("Apartment is not available or booking conflict");

        var booking = Booking.Create(userId, apartmentId, range);

        await _bookingRepository.AddAsync(booking);

        return booking;
    }
}