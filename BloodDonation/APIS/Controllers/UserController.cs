using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Services.Interfaces;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("Get-All-User")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _userService.GetAllUsersAsync();

                if (!users.Any())
                {
                    return NoContent();
                }

                var response = new
                {
                    totalUsers = users.Count(),
                    users = users.Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        u.Email,
                        u.FullName,
                        u.Phone,
                        u.UserIdCard,
                        DateOfBirth = u.DateOfBirth?.ToString("yyyy-MM-dd"),
                        u.Role
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }
        [HttpGet("Get-User-By-Role/{role}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetUsersByRole([FromRoute] string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest(new { message = "Role cannot be empty" });
                }

                if (!Enum.TryParse<UserRoles>(role, true, out UserRoles userRole))
                {
                    return BadRequest(new
                    {
                        message = "Invalid role",
                        validRoles = Enum.GetNames(typeof(UserRoles))
                    });
                }

                var users = await _userService.GetUsersByRoleAsync(userRole.ToString());
                if (!users.Any())
                {
                    return NoContent();
                }

                var response = new
                {
                    role = userRole.ToString(),
                    totalUsers = users.Count(),
                    users = users.Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        u.Email,
                        u.FullName,
                        u.Phone,
                        u.UserIdCard,
                        DateOfBirth = u.DateOfBirth?.ToString("yyyy-MM-dd"),
                        u.Role
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by role: {Role}", role);
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("Search-User-By-Name")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term cannot be empty" });
                }

                var users = await _userService.SearchUsersByNameAsync(searchTerm);
                if (!users.Any())
                {
                    return NoContent();
                }

                var response = new
                {
                    searchTerm = searchTerm,
                    totalResults = users.Count(),
                    users = users.Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        u.Email,
                        u.FullName,
                        u.Phone,
                        u.UserIdCard,
                        DateOfBirth = u.DateOfBirth?.ToString("yyyy-MM-dd"),
                        u.Role
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new { message = "An error occurred while searching users" });
            }
        }
    }
}