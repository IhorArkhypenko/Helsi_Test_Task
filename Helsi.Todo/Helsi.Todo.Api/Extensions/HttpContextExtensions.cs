namespace Helsi.Todo.Api.Extensions;

public static class HttpContextExtensions
{
    public static bool TryGetUserId(this HttpContext httpContext, out Guid userId)
    {
        if (Guid.TryParse(httpContext.Request.Headers["X-User-Id"], out var id))
        {
            userId = id;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }
}