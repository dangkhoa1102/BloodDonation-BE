using Blood_Donation_System.DTOs.User;
using Blood_Donation_System.DTOs.Auth;
using Blood_Donation_System.Models;
using Blood_Donation_System.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Blood_Donation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(UserCreateDTO userCreateDto)
        {
            if (await _authService.UserExistsAsync(userCreateDto.Username, userCreateDto.Email))
            {
                return BadRequest("Tài khoản hoặc email đã tồn tại");
            }

            var user = await _authService.RegisterAsync(userCreateDto);
            
            if (user == null)
            {
                return BadRequest("Đăng ký thất bại");
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

            return CreatedAtAction(nameof(Register), userDto);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserLoginDTO loginDto)
        {
            var (user, token) = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            
            if (user == null)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng");
            }

            var response = new AuthResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = token
            };

            return Ok(response);
        }
    }
} 