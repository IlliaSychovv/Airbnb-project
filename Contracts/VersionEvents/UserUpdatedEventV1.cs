namespace Contracts.VersionEvents;

public record UserUpdatedEventV1
{
    public Guid Id { get; set; }
    public string Name { get; set; }  
    public string Email { get; set; }  
    public string PhoneNumber { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version => 1;
}