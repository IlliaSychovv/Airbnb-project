using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IApartmentRepository
{
    Task<PagedResult<Apartment>> GetAllApartmentsAsync(int pageNumber, int pageSize, string? location = null);
    Task<PagedResult<Apartment>> GetAvailableApartmentsAsync(DateRange range, int pageNumber, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null);
    Task<PagedResult<Apartment>> GetAllApartmentsWithBookingsAsync(int pageNumber, int pageSize);
    Task AddAsync(Apartment apartment);
}