namespace Airbnb.Application.DTOs;

public class BookingDto
{
    public Guid UserId { get; set; }
    public Guid ApartmentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}