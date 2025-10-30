namespace Contracts.MonolithEvents;

public record ApartmentUpdatedEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public string ChangeType { get; set; } = "Upsert";
}