using Blood_Donation_System.DTOs;
using Blood_Donation_System.DTOs.User;
using Blood_Donation_System.Models;
using Blood_Donation_System.Services;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blood_Donation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                UserIdCard = u.UserIdCard,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                DateOfBirth = u.DateOfBirth,
                Role = u.Role
            });
            
            return Ok(userDtos);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            // Kiểm tra nếu người dùng chỉ có thể xem thông tin của chính mình hoặc là Admin/Staff
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id.ToString() && 
                !User.IsInRole(Role.Admin) && 
                !User.IsInRole(Role.Staff))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserIdCard = user.UserIdCard,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role
            };

            return Ok(userDto);
        }

        // GET: api/Users/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUserProfile()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out Guid userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserIdCard = user.UserIdCard,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role
            };

            return Ok(userDto);
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> CreateUser(UserCreateDTO userCreateDto)
        {
            var user = new User
            {
                Username = userCreateDto.Username,
                Password = userCreateDto.Password, // In a real application, you should hash this password
                UserIdCard = userCreateDto.UserIdCard,
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                Phone = userCreateDto.Phone,
                DateOfBirth = userCreateDto.DateOfBirth,
                Role = Role.Customer // Default role
            };

            var result = await _userService.AddUserAsync(user);

            if (!result)
            {
                return BadRequest("Username or email already exists");
            }

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserIdCard = user.UserIdCard,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
        }
         // PATCH: api/Users/5/role
        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleDTO changeRoleDto)
        {
            if (string.IsNullOrEmpty(changeRoleDto.Role))
            {
                return BadRequest("Role is required");
            }

            // Validate that the role is one of the allowed values
            if (changeRoleDto.Role != Role.Admin && 
                changeRoleDto.Role != Role.Staff && 
                changeRoleDto.Role != Role.Customer)
            {
                return BadRequest("Invalid role value");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = changeRoleDto.Role;
            var result = await _userService.UpdateUserAsync(user);

            if (!result)
            {
                return BadRequest("Failed to update user role");
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(UserLoginDTO userLoginDto)
        {
            var user = await _userService.GetUserByEmailAsync(userLoginDto.Email);

            if (user == null || user.Password != userLoginDto.Password) // In a real app, you'd use password hashing
            {
                return Unauthorized("Invalid email or password");
            }

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserIdCard = user.UserIdCard,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role
            };

            return Ok(userDto);
        }
    }

    public class ChangeRoleDTO
    {
        public string Role { get; set; }
    }
} 