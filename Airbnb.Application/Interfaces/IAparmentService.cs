using Airbnb.Domain.Entities;
using Airbnb.Application.DTOs;

namespace Airbnb.Application.Interfaces;

public interface IApartmentService
{
    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto);
    Task<PagedResponse<Apartment>> GetPagedApartmentsAsync(int pageNumber, int pageSize, string? location = null);
}