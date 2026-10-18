using Microsoft.AspNetCore.Identity;

namespace Project.Api.Entities;

public class ApplicationUser : IdentityUser
{
    // Indicates whether the account is blocked by an administrator.
    public bool IsBlocked { get; set; } = false;
}
