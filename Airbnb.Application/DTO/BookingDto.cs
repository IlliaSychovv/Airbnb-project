namespace Airbnb.Application.DTO;

public record BookingDto
{
    public Guid UserId { get; set; }
    public Guid ApartmentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}