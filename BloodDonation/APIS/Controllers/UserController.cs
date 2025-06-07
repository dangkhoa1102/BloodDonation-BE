using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BloodDonationSupportContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(BloodDonationSupportContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "This is a public endpoint that anyone can access" });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "User ID not found in token" });
                }

                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Don't return sensitive information
                return Ok(new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.Phone,
                    user.DateOfBirth,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving the profile" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "This endpoint is only accessible by administrators" });
        }

        [Authorize]
        [HttpGet("role")]
        public IActionResult GetUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { role = role ?? "No role assigned" });
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetUserClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new { claims });
        }
    }
} 