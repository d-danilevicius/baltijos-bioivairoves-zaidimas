using System.Security.Claims;

namespace Project.Api.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string GetRequiredUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("The signed-in user has no identifier claim.");
}
