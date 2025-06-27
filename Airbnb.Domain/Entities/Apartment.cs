namespace Airbnb.Domain.Entities;

public class Apartment
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
    public string ExternalId { get; set; }
    
    public ICollection<Booking> Bookings { get; set; }
}