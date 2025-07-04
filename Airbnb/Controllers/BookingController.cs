using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Airbnb.Application.Services;
using Airbnb.Application.DTOs;
using Airbnb.Domain.ValueObject;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingAppService _bookingAppService;
 
    public BookingController(BookingAppService bookingService)
    {
        _bookingAppService = bookingService;
    }

    [HttpPost("book")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
    {
        var dataRenge = new DateRange(dto.StartDate, dto.EndDate);
 
        var bookingId = await _bookingAppService.CreateBookingAsync(dto.ApartmentId, dto.UserId, dataRenge);
        return Ok(new { Booking = bookingId });
    }
    
    [Authorize(Roles = "Client")]
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