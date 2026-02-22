namespace AffirmationGenerator.Server.Api.Extensions;

public static class HttpContextExtensions
{
    extension(HttpContext httpContext)
    {
        public string? GetClientIpFromHeaderOrDefault(string headerName) =>
            httpContext.Request.Headers.TryGetValue(headerName, out var clientIp) == false
                ? httpContext.Connection.RemoteIpAddress?.ToString()
                : clientIp;
    }
}
