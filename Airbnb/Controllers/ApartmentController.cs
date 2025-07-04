using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
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
    public async Task<IActionResult> CreateApartment(CreateApartmentDto apartmentDto)
    {
        var apartment = await _apartmentService.CreateApartmentAsync(apartmentDto);
        return Created(string.Empty, apartment);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<Apartment>>> GetAllApartments([FromQuery] int pageNumber = 1 ,
        [FromQuery] int pageSize = 10,
        string? location = null)
    {
        var pagedResult = await _apartmentService.GetPagedApartmentsAsync(pageNumber, pageSize, location);
        return Ok(pagedResult);
    }
}