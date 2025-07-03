namespace Airbnb.Application.DTOs.Migrations;

public record ExternalApartment
{
    public string ExternalId { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
}