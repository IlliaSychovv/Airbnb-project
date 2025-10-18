using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Airbnb.Domain.ValueObject;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Controllers;

[ApiController]
//[Authorize(Roles = "Client")]
[Route("api/v1/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IBookingSagaOrchestrator _bookingSagaOrchestrator;
 
    public BookingController(IBookingService bookingService, IBookingSagaOrchestrator bookingSagaOrchestrator)
    {
        _bookingService = bookingService;
        _bookingSagaOrchestrator = bookingSagaOrchestrator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
    {
        var bookingId = await _bookingSagaOrchestrator.CreateSagaBooking(
            dto.ApartmentId, dto.UserId, 
            new DateRange(dto.StartDate, dto.EndDate), 
            dto.Amount, dto.AccountNumber);
        
        if (bookingId == null)
            return BadRequest(new { Message = "Booking failed and cancelled!" });

        return Ok(new { BookingId = bookingId });
    }
    
    [HttpGet] 
    public async Task<IActionResult> GetClientBookings(Guid userGuid)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userGuid);
        return Ok(bookings);
    }
}