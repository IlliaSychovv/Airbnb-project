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
    private readonly IBookingAppService _bookingAppService;
    private readonly IBookingSagaOrchestrator _bookingSagaOrchestrator;
 
    public BookingController(IBookingAppService bookingService, IBookingSagaOrchestrator bookingSagaOrchestrator)
    {
        _bookingAppService = bookingService;
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
            return BadRequest(new { Success = false, Message = "Payment failed and booking cancelled!" });

        return Ok(new { Success = true, BookingId = bookingId });
    }
    
    [HttpGet] 
    public async Task<IActionResult> GetClientBookings(Guid userGuid)
    {
        var bookings = await _bookingAppService.GetUserBookingsAsync(userGuid);
        return Ok(bookings);
    }
}