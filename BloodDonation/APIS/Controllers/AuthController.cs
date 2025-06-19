using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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
    }
}