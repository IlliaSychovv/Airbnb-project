using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Mapster;
using MapsterMapper;
using SequentialGuid;

namespace Airbnb.Infrastructure.Services;

public class ApartmentService : IApartmentService
{
    private readonly AppDbContext _context;
    private readonly IApartmentRepository _apartmentRepository;

    public ApartmentService(AppDbContext context, IApartmentRepository apartmentRepository)
    {
        _context = context;
        _apartmentRepository = apartmentRepository;
    }

    public async Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto)
    {
        var apartment = dto.Adapt<Apartment>(); 
        apartment.Id = SequentialGuidGenerator.Instance.NewGuid();
        
        await _context.Apartments.AddAsync(apartment);
        await _context.SaveChangesAsync();
        
        return apartment.Adapt<ApartmentDto>();
    }

    public async Task<PagedResponse<Apartment>> GetPagedApartmentsAsync(int pageNumber, int pageSize,
        string? location = null)
    {
        var item = await _apartmentRepository.GetAsync(pageNumber, pageSize, location);
        var totalcount = await _apartmentRepository.GetTotalCountAsync(location);

        return new PagedResponse<Apartment>
        {
            Items = item.ToList(),
            TotalCount = totalcount,
            PageSize = pageSize
        };
    }
}