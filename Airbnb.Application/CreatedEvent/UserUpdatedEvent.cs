namespace Airbnb.Application.CreatedEvent;

public record UserUpdatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }  
    public string Email { get; set; }  
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}