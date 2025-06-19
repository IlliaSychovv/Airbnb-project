using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Mapster;
using SequentialGuid;

namespace Airbnb.Infrastructure.Services;

public class ApartmentService : IApartmentService
{
    private readonly AppDbContext _context;

    public ApartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto)
    {
        var apartment = dto.Adapt<Apartment>(); 
        apartment.Id = SequentialGuidGenerator.Instance.NewGuid();
        
        await _context.Apartments.AddAsync(apartment);
        await _context.SaveChangesAsync();
        
        return apartment.Adapt<ApartmentDto>();
    }
}