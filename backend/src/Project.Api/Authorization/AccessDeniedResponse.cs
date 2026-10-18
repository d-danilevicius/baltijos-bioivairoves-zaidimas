using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project.Api.Authorization;

public static class AccessDeniedResponse
{
    public const string ErrorCode = "AccessDenied";
    public const string Message = "Prieiga uždrausta";

    public static Task WriteAsync(HttpResponse response)
    {
        var details = new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            [""] = new[] { ErrorCode }
        })
        {
            Status = StatusCodes.Status403Forbidden,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            Title = Message,
            Detail = Message
        };

        response.StatusCode = StatusCodes.Status403Forbidden;
        return response.WriteAsJsonAsync(details, options: null, contentType: "application/problem+json");
    }
}
