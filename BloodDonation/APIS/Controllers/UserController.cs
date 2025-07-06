using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Enums;
using System.Security.Claims;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]

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
        [HttpPut("Update-User/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDTO updateDto)
        {
            try
            {
                var (success, message) = await _userService.UpdateUserAsync(id, updateDto);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user" });
            }
        }
        [HttpGet("Get-User-Detail/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]

        public async Task<IActionResult> GetUserDetail(Guid id)
        {
            try
            {
                var user = await _userService.GetUserDetailAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var response = new
                {
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    fullName = user.FullName,
                    phone = user.Phone,
                    userIdCard = user.UserIdCard,
                    dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                    role = user.Role,                   
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user detail for ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving user detail" });
            }
        }
        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user identification" });
                }

                var user = await _userService.GetCurrentUserAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                return StatusCode(500, new { message = "An error occurred while retrieving user information" });
            }
        }

        [HttpGet("search-by-fullname/{fullName}")]
        [Authorize]
        public async Task<IActionResult> GetUsersByFullName(string fullName)
        {
            try
            {
                if (!User.IsInRole("Admin") && !User.IsInRole("Staff"))
                {
                    return Unauthorized(new { message = "Unauthorized access" });
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    return BadRequest(new { message = "Full name cannot be empty" });
                }

                var users = await _userService.GetUsersByFullNameAsync(fullName);
                if (!users.Any())
                {
                    return NoContent();
                }

                var response = new
                {
                    searchTerm = fullName,
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
                _logger.LogError(ex, "Error searching users by full name: {FullName}", fullName);
                return StatusCode(500, new { message = "An error occurred while searching users" });
            }
        }
        [HttpGet("get-by-idcard/{userIdCard}")]
        [Authorize] 
        public async Task<IActionResult> GetByUserIdCard(string userIdCard)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userIdCard))
                {
                    return BadRequest(new { message = "UserIdCard cannot be empty" });
                }

                var user = await _userService.GetByUserIdCardAsync(userIdCard);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var response = new
                {
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    fullName = user.FullName,
                    phone = user.Phone,
                    userIdCard = user.UserIdCard,
                    dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                    role = user.Role
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by UserIdCard: {UserIdCard}", userIdCard);
                return StatusCode(500, new { message = "An error occurred while retrieving user information" });
            }
        }
    }
}