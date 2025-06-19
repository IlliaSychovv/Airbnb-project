using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
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
        await _apartmentService.CreateApartmentAsync(apartmentDto);
        return NoContent();
    }
}