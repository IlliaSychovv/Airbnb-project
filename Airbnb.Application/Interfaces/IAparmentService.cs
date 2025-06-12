using Airbnb.Domain.Entities;
using Airbnb.Application.DTOs;

namespace Airbnb.Application.Interfaces;

public interface IApartmentService
{
    Task<ApartmentDto> CreateApartmentAsync(ApartmentDto dto);
}