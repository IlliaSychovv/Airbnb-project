using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Mapster;
using SequentialGuid;

namespace Airbnb.Application.Services;

public class ApartmentService : IApartmentService
{ 
    private readonly IApartmentRepository _apartmentRepository;

    public ApartmentService(IApartmentRepository apartmentRepository)
    {
         _apartmentRepository = apartmentRepository;
    }

    public async Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto)
    {
        var apartment = dto.Adapt<Apartment>(); 
        apartment.Id = SequentialGuidGenerator.Instance.NewGuid();
        apartment.ExternalId = Guid.NewGuid().ToString();
        apartment.Metadata = "{}";
        
        await _apartmentRepository.AddAsync(apartment);
        
        return apartment.Adapt<ApartmentDto>();
    }

    public async Task<PagedResponse<Apartment>> GetPagedApartmentsAsync(int pageNumber, int pageSize,
        string? location = null)
    {
        var item = await _apartmentRepository.GetAsync(pageNumber, pageSize, location);
        var totalCount = await _apartmentRepository.GetTotalCountAsync(location);

        return new PagedResponse<Apartment>
        {
            Items = item.ToList(),
            TotalCount = totalCount,
            PageSize = pageSize
        };
    }
}