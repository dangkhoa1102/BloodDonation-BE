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


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("process/by-donor/{donorId}")]
        public async Task<IActionResult> GetDonationProcessesByDonorId(Guid donorId)
        {
            var result = await _appointmentService.GetLatestDonationProcessByDonorIdAsync(donorId);
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
