namespace AffirmationGenerator.Server.Application.Extensions;

public static class HttpContextExtensions
{
    extension(HttpContext httpContext)
    {
        public string? GetClientIpAddress(string headerName) =>
            httpContext.Request.Headers.TryGetValue(headerName, out var clientIp) == false
                ? httpContext.Connection.RemoteIpAddress?.ToString()
                : clientIp;
    }
}
