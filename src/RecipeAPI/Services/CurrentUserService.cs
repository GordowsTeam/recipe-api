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

    public string? GetUserId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        // Prefer JWT "sub" claim when available (e.g. after adding JWT Bearer auth)
        var sub = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(sub))
            return sub;

        // Fallback: client sends userId in header (e.g. from decoded Cognito id_token)
        return context.Request.Headers["X-User-Id"].FirstOrDefault();
    }
}
