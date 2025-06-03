using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Entities; 

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}