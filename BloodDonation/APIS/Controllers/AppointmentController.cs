using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // Staff tạo lịch hẹn hiến máu
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _appointmentService.CreateAppointmentAsync(dto);
            return Ok(result);
        }

        // Lấy thông tin lịch hẹn theo ID
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{donationId}")]
        public async Task<IActionResult> GetAppointment(Guid donationId)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(donationId);
            if (result == null)
                return NotFound(new { message = "Appointment not found" });
            return Ok(result);
        }

        // Staff cập nhật ngày hẹn hiến máu
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        [HttpPut("update-donation-date")]
        public async Task<IActionResult> UpdateDonationDate([FromBody] UpdateDonationDateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _appointmentService.UpdateDonationDateAsync(dto);
            if (!result)
                return NotFound(new { message = "Blood donation not found" });

            return Ok(new { message = "Donation date updated successfully" });
        }
    }
}
