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
        var booking = Booking.Create(userId, apartmentId, range);
        
        bool hasConflict = await _bookingRepository.ExistsConflictAsync(apartmentId, range);
        if (hasConflict)
        {
            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.AddAsync(booking);
            throw new InvalidOperationException("Apartment is not available");
        }
        
        booking.Status = BookingStatus.PendingPayment;
        await _bookingRepository.AddAsync(booking);
        _bookingsCounter.Add(1);

        return booking;
    }

    public async Task MarkAsPaid(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        booking.Status = BookingStatus.Approved;
        await _bookingRepository.UpdateAsync(booking);
    }

    public async Task MarkAsCancelled(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        
        booking.Status = BookingStatus.Cancelled;
        await _bookingRepository.UpdateAsync(booking);
    }
}