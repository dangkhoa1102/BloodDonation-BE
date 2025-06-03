using Blood_Donation_System.DTOs;
using Blood_Donation_System.DTOs.AuthDTOs;
using Blood_Donation_System.DTOs.UserDTOs;
using Blood_Donation_System.Models;
using Blood_Donation_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
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

        // POST: api/Auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authService.LogoutAsync(token);
            
            if (!result)
            {
                return BadRequest("Đăng xuất thất bại");
            }

            return Ok("Đăng xuất thành công");
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            
            if (!result)
            {
                return BadRequest("Email không tồn tại trong hệ thống");
            }

            return Ok("Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn");
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (!await _authService.ValidateResetTokenAsync(resetPasswordDto.Token))
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");
            }

            var result = await _authService.ResetPasswordAsync(
                resetPasswordDto.Token,
                resetPasswordDto.NewPassword
            );

            if (!result)
            {
                return BadRequest("Đặt lại mật khẩu thất bại");
            }

            return Ok("Đặt lại mật khẩu thành công");
        }

        // POST: api/Auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return Unauthorized("Người dùng chưa đăng nhập");
            }

            var result = await _authService.ChangePasswordAsync(
                userGuid,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword
            );

            if (!result)
            {
                return BadRequest("Mật khẩu hiện tại không đúng");
            }

            return Ok("Đổi mật khẩu thành công");
        }
    }

    public class AuthResponseDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
} 