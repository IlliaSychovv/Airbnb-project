using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.Services;

public interface IApartmentService
{
    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto);
    Task<PagedResponse<Apartment>> GetPagedApartmentsAsync(int pageNumber, int pageSize, string? location = null);
    Task<PagedResponse<ApartmentDto>> GetAllApartmentsWithBookings(int pageNumber, int pageSize);
    Task<PagedResponse<ApartmentDto>> GetAvailableApartments(DateRange range, int pageNumber, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null);
}