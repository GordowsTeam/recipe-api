using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserEmail()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        var fromJwt = context.User?.FindFirst("email")?.Value
            ?? context.User?.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(fromJwt))
            return fromJwt;

        return context.Request.Headers["X-User-Email"].FirstOrDefault();
    }

    public string? GetCognitoUsername()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        var fromJwt = context.User?.FindFirst("cognito:username")?.Value;
        if (!string.IsNullOrEmpty(fromJwt))
            return fromJwt;

        return context.Request.Headers["X-Cognito-Username"].FirstOrDefault();
    }
}
