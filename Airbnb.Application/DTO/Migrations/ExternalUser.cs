namespace Airbnb.Application.DTO.Migrations;

public record ExternalUser
{
    public string ExternalId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}