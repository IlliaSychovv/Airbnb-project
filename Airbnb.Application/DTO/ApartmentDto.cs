namespace Airbnb.Application.DTO;

public record ApartmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
}