using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services;

namespace APIS.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
        {   
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request Path: {Path}", context.Request.Path);

                // Skip validation for these paths
                if (context.Request.Path.StartsWithSegments("/swagger") ||
                    context.Request.Path.StartsWithSegments("/api/Auth/login") ||
                    context.Request.Path.StartsWithSegments("/api/Auth/register"))
                {
                    await _next(context);
                    return;
                }

                var authHeader = context.Request.Headers["Authorization"].ToString();
                _logger.LogInformation("Auth Header: {Header}", authHeader);

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Invalid Authorization header format" });
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token is missing" });
                    return;
                }

                if (AuthService.IsTokenBlacklisted(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                    return;
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TokenValidationMiddleware");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
            }
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}
