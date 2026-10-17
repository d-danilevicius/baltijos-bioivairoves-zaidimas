using Microsoft.AspNetCore.Identity;

namespace Project.Api.Entities;

public class ApplicationUser : IdentityUser
{
    // IdentityUser already contains Id, Email, UserName and PasswordHash.
    // Additional properties are not required by the specification.
}
