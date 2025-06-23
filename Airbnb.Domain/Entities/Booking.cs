using Airbnb.Domain.ValueObject;

namespace Airbnb.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ApartmentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime EndBookingDate { get; set; }
    public BookingStatus Status { get; set; }
    
    public static Booking Create(Guid userId, Guid apartmentId, DateRange range)
    {
        return new Booking
        {
            Id = Guid.NewGuid(), 
            UserId = userId,
            ApartmentId = apartmentId,
            BookingDate = range.Start,
            EndBookingDate = range.End,
            Status = BookingStatus.Pending
        };
    }
}

public enum BookingStatus
{
    Pending,
    Approved,
    Cancelled
}