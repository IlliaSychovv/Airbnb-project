using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces;

public interface IApartmentRepository
{
    Task<IReadOnlyList<Apartment>> GetAsync(int pageNumber, int pageSize, string? location = null);
    Task<int> GetTotalCountAsync(string? location = null);
    Task<Apartment> GetByIdAsync(Guid apartmentId);
    Task<List<Apartment>> GetAvailableApartmentsAsync(DateRange range);
    Task AddAsync(Apartment apartment);
}