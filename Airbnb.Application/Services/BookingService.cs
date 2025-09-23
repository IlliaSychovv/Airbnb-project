using System.Diagnostics.Metrics;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Services;

public class BookingService : IBookingService
{
    private static readonly Meter _meter = new("BookingService.Metrics", "1.0");
    private static readonly Counter<long> _bookingsCounter =
        _meter.CreateCounter<long>("business_bookings_created_total", description: "Total number of bookings created");
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
        _bookingsCounter.Add(1);

        return booking;
    }
}