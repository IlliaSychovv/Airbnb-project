using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/apartments")]
public class ApartmentController : ControllerBase
{
    private readonly IApartmentService _apartmentService;

    public ApartmentController(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }
    
    [HttpPost]
    //[Authorize(Roles = "Host")] 
    public async Task<IActionResult> CreateApartment([FromBody] CreateApartmentDto apartmentDto)
    {
        var apartment = await _apartmentService.CreateApartmentAsync(apartmentDto);
        return Created(string.Empty, apartment);
    }

    [HttpGet] 
    //[Authorize(Roles = "Client")]
    public async Task<ActionResult<PagedResponse<Apartment>>> GetAllApartments([FromQuery] int pageNumber = 1 ,
        [FromQuery] int pageSize = 10,
        string? location = null)
    {
        var pagedResult = await _apartmentService.GetPagedApartmentsAsync(pageNumber, pageSize, location);
        return Ok(pagedResult);
    }

    [HttpGet("reserved")]
    public async Task<ActionResult<PagedResponse<ApartmentDto>>> GetReservedApartments([FromQuery] int pageNumber = 1 , [FromQuery] int pageSize = 10)
    {
        var reservedApartments = await _apartmentService.GetAllApartmentsWithBookings(pageNumber, pageSize);
        return Ok(reservedApartments);
    }

    [HttpGet("available")]
    public async Task<ActionResult<PagedResponse<ApartmentDto>>> GetAvailableApartments([FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, [FromQuery] int pageNumber = 1 , [FromQuery] int pageSize = 10,
        [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
    {
        var range = new DateRange(startDate, endDate);
        var availableApartments = await _apartmentService.GetAvailableApartments(range, pageNumber, pageSize, minPrice, maxPrice);
        
        return Ok(availableApartments);
    }
}