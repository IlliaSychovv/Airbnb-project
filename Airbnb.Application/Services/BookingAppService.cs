using Airbnb.Domain.Services;
using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Services;

public class BookingAppService : IBookingAppService
{ 
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingService _bookingService;

    public BookingAppService(IBookingRepository bookingRepository, IBookingService bookingService)
    {
         _bookingRepository = bookingRepository;
         _bookingService = bookingService;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(Guid userId)
    {
        return await _bookingRepository.GetByUserIdAsync(userId);
    }

    public async Task<Guid> CreateBookingAsync(Guid userId, Guid apartmentId, DateRange range)
    {
        var booking = await _bookingService.CreateBooking(apartmentId, userId, range);
        return booking.Id; 
    }
}