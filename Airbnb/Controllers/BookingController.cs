using System.Security.Claims;
using Airbnb.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using Airbnb.Domain.ValueObject;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Airbnb.Controllers;

[ApiController]
[Authorize(Roles = "Client")]
[Route("api/v1/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingAppService _bookingAppService;
 
    public BookingController(IBookingAppService bookingService)
    {
        _bookingAppService = bookingService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
    {
        var dataRenge = new DateRange(dto.StartDate, dto.EndDate);
 
        var bookingId = await _bookingAppService.CreateBookingAsync(dto.ApartmentId, dto.UserId, dataRenge);
        return Ok(new { Booking = bookingId });
    }
    
    [HttpGet] 
    public async Task<IActionResult> GetClientBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();
        
        var userGuid = Guid.Parse(userId);
        
        var bookings = await _bookingAppService.GetUserBookingsAsync(userGuid);
        return Ok(bookings);
    }
}