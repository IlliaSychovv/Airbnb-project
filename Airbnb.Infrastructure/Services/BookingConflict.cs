using Airbnb.Application.Interfaces;
using Airbnb.Data;
using Airbnb.Domain.DomainInterfaces;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Infrastructure.Services;

public class BookingConflict : IBookingConflict
{
    private readonly IBookingRepository _bookingRepository;

    public BookingConflict(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<bool> HasConflictAsync(Guid apartmentId, DateRange range)
    {
        return await _bookingRepository.ExistsConflictAsync(apartmentId, range);
    }
}