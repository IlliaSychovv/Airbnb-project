using Airbnb.Application.DTO;
using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces.Services;

public interface IApartmentService
{
    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto);
    Task<PagedResponse<Apartment>> GetPagedApartmentsAsync(int pageNumber, int pageSize, string? location = null);
}