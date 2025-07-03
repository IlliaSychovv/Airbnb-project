namespace Airbnb.Application.DTOs;

public record CreateApartmentDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
}