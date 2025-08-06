using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Entities; 

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}