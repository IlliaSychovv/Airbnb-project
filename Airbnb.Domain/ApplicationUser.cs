using Microsoft.AspNetCore.Identity;

namespace Airbnb.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}