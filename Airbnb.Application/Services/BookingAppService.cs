using Airbnb.Domain.Services;
using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Services;

public class BookingAppService
{
    private readonly BookingService _bookingService;
    private readonly IBookingRepository _bookingRepository;

    public BookingAppService(
        BookingService bookingService,
        IBookingRepository bookingRepository)
    {
        _bookingService = bookingService;
        _bookingRepository = bookingRepository;
    }

    public async Task<Guid> CreateBookingAsync(Guid userId, Guid apartmentId, DateRange range)
    {
        var booking = await _bookingService.CreateBooking(userId, apartmentId, range);

        await _bookingRepository.AddAsync(booking);  
        return booking.Id;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(Guid userId)
    {
        return await _bookingRepository.GetByUserIdAsync(userId);
    }
}