using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Entities; 

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
}