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
                // Skip validation for these paths
                if (context.Request.Path.StartsWithSegments("/swagger") ||
                    context.Request.Path.StartsWithSegments("/api/Auth/login") ||
                    context.Request.Path.StartsWithSegments("/api/Auth/register"))
                {
                    await _next(context);
                    return;
                }

                var token = context.Request.Headers["Authorization"].ToString();
                
                // Nếu token đã có "Bearer" thì giữ nguyên, nếu không thì thêm vào
                if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer "))
                {
                    token = $"Bearer {token}";
                    // Update lại header với token mới
                    context.Request.Headers["Authorization"] = token;
                }

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token is missing" });
                    return;
                }

                var actualToken = token.Replace("Bearer ", "").Trim();
                if (AuthService.IsTokenBlacklisted(actualToken))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token validation");
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
