using Airbnb.Application.Interfaces;
using Airbnb.Domain.DomainInterfaces;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Infrastructure.Services;

public class ApartmentAvailability : IApartmentAvailability
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingConflict _bookingConflict;

    public ApartmentAvailability(IApartmentRepository apartmentRepository, IBookingConflict bookingConflict)
    {
        _apartmentRepository = apartmentRepository;
        _bookingConflict = bookingConflict;
    }

    public async Task<bool> IsAvailableAsync(Guid apartmentId, DateRange range)
    {
        var apartment = await _apartmentRepository.GetByIdAsync(apartmentId);
        if (apartment == null)
            return false;
        
        var conflicts = await _bookingConflict.HasConflictAsync(apartmentId, range);
        return !conflicts;
    }
}