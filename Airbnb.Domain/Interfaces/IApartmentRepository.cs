using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces;

public interface IApartmentRepository
{
    Task<List<Apartment>> GetAllAsync();
    Task<Apartment> GetByIdAsync(Guid apartmentId);
    Task<List<Apartment>> GetAvailableApartmentsAsync(DateRange range);
}