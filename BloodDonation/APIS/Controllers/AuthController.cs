using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try 
            {
                if (loginDto == null)
                {
                    return BadRequest(new { message = "Invalid request data" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Email and password are required" });                    
                }

                _logger.LogInformation("Attempting login for email: {Email}", loginDto.Email);

                var (success, message, token) = await _authService.LoginAsync(loginDto);

                if (!success)
                {
                    _logger.LogWarning("Login failed for email: {Email}. Reason: {Message}", 
                        loginDto.Email, message);
                    return BadRequest(new { message });
                }

                _logger.LogInformation("Login successful for email: {Email}", loginDto.Email);
                return Ok(new { token, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto?.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                if (registerDto == null)
                {
                    return BadRequest(new { message = "Invalid request data" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    return BadRequest(new { message = "Passwords do not match" });
                }

                var (success, message) = await _authService.RegisterAsync(registerDto);

                if (!success)
                {
                    return BadRequest(new { message });
                }

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", 
                    registerDto?.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString()
                    .Replace("Bearer ", "").Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "No token provided" });
                }

                var (success, message) = await _authService.LogoutAsync(token);

                if (!success)
                {
                    return BadRequest(new { message });
                }

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in logout endpoint");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        // ====== GOOGLE LOGIN API ======
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                // Redirect về FE với lỗi xác thực Google
                return Redirect("http://localhost:5173/register?error=GoogleAuthFailed");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Redirect("http://localhost:5173/register?error=NoEmail");
            }

            var user = await _authService.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Chưa có tài khoản, FE sẽ xử lý đăng ký
                var registerUrl = $"http://localhost:5173/register?needRegister=true&email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name ?? "")}";
                return Redirect(registerUrl);
            }

            var (success, message, token) = await _authService.LoginWithGoogleAsync(email, name);

            if (!success)
            {
                return Redirect($"http://localhost:5173/register?error={Uri.EscapeDataString(message)}");
            }

            // Đã có tài khoản, redirect về FE kèm token
            var successUrl = $"http://localhost:5173/register?needRegister=false&email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name ?? "")}&token={Uri.EscapeDataString(token)}";
            return Redirect(successUrl);
        }



    }
}