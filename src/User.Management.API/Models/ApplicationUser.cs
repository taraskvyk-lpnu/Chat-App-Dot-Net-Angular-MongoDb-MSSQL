using Microsoft.AspNetCore.Identity;

namespace User.Management.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}