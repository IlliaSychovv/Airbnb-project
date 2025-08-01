namespace Airbnb.Application.DTOs.Authorization;

public record UpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }  
    public string Email { get; set; }  
    public string PhoneNumber { get; set; }
}