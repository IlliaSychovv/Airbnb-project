using Microsoft.AspNetCore.Mvc;
using Airbnb.Application.Services;
using Airbnb.Application.DTOs;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingAppService _bookingService;

    public BookingController(BookingAppService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("book")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
    {
        var dataRenge = new DateRange(dto.StartDate, dto.EndDate);

        try
        {
            var bookingId = await _bookingService.CreateBookingAsync(dto.ApartmentId, dto.UserId, dataRenge);
            return Ok(new { Booking = bookingId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBookings(Guid userId)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }
}