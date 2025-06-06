using Microsoft.AspNetCore.Http;
using Services;

namespace APIS.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip token validation for login and register endpoints
            if (context.Request.Path.StartsWithSegments("/api/Auth/login") || 
                context.Request.Path.StartsWithSegments("/api/Auth/register"))
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token) && AuthService.IsTokenBlacklisted(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                    return;
                }
            }

            await _next(context);
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
