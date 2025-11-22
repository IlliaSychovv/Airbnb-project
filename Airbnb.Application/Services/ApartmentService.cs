using System.Diagnostics.Metrics;
using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Mapster;
using SequentialGuid;

namespace Airbnb.Application.Services;

public class ApartmentService : IApartmentService
{
    private static readonly Meter _meter = new("ApartmentService.Metrics", "1.0");
    private static readonly Counter<long> _apartmentsRequestedCounter =
        _meter.CreateCounter<long>("business_apartments_requested_total", description: "Total number of times apartments were requested");

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
        _apartmentsRequestedCounter.Add(1);
        
        var pagedResult = await _apartmentRepository.GetAllApartmentsAsync(pageNumber, pageSize, location);

        return new PagedResponse<Apartment>
        {
            Items = pagedResult.Items.ToList(),
            TotalCount = pagedResult.TotalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    public async Task<PagedResponse<ApartmentDto>> GetAllApartmentsWithBookings(int pageNumber, int pageSize)
    {
        var pagedResult = await _apartmentRepository.GetAllApartmentsWithBookingsAsync(pageNumber, pageSize);

        return new PagedResponse<ApartmentDto>
        {
            Items = pagedResult.Items.Select(a => a.Adapt<ApartmentDto>()).ToList(),
            TotalCount = pagedResult.TotalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    public async Task<PagedResponse<ApartmentDto>> GetAvailableApartments(DateRange range, int pageNumber, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        var pagedResult = await _apartmentRepository.GetAvailableApartmentsAsync(range, pageNumber, pageSize, minPrice,  maxPrice);

        return new PagedResponse<ApartmentDto>
        {
            Items = pagedResult.Items.Select(a => a.Adapt<ApartmentDto>()).ToList(),
            TotalCount = pagedResult.TotalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }
}