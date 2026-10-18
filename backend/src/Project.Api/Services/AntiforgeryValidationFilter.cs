using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Api.Services;

// Validates antiforgery token for unsafe HTTP methods, skipping endpoints marked with IgnoreAntiforgeryTokenAttribute
public class AntiforgeryValidationFilter : IAsyncAuthorizationFilter
{
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryValidationFilter(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var req = context.HttpContext.Request;

        // Skip safe methods
        if (HttpMethods.IsGet(req.Method) || HttpMethods.IsHead(req.Method) || HttpMethods.IsOptions(req.Method) || HttpMethods.IsTrace(req.Method))
            return;

        // Skip endpoints that opted out
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IgnoreAntiforgeryTokenAttribute>() != null)
            return;

        try
        {
            await _antiforgery.ValidateRequestAsync(context.HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            var details = new ValidationProblemDetails();
            details.Errors.Add("", new[] { "InvalidCsrfToken" });
            context.Result = new BadRequestObjectResult(details);
        }
    }
}
