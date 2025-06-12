using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[ApiController]
[Route("/api/apartment")]
public class ApartmentController : ControllerBase
{
    private readonly IApartmentService _apartmentService;

    public ApartmentController(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApartmentDto>> CreateApartment(ApartmentDto apartmentDto)
    {
        var createdApartment = await _apartmentService.CreateApartmentAsync(apartmentDto);
        return Ok(createdApartment);
    }
}