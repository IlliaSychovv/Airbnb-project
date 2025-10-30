using Airbnb.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Entities; 

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}