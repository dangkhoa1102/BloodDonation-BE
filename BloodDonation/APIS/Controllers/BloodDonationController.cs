using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodDonationController : ControllerBase
    {
        private readonly IBloodDonationService _service;
        private readonly ILogger<BloodDonationController> _logger; // Thêm logger

        public BloodDonationController(
            IBloodDonationService service,
            ILogger<BloodDonationController> logger) // Cập nhật constructor
        {
            _service = service;
            _logger = logger; // Khởi tạo logger
        }     

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodDonationDto>>> GetAll()
        {
            var donations = await _service.GetAllAsync();
            return Ok(donations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BloodDonationDto>> GetById(Guid id)
        {
            var donation = await _service.GetByIdAsync(id);
            if (donation == null)
                return NotFound();
            return Ok(donation);
        }

        [HttpGet("donor/{donorId}")]
        public async Task<ActionResult<IEnumerable<BloodDonationDto>>> GetByDonorId(Guid donorId)
        {
            var donations = await _service.GetByDonorIdAsync(donorId);
            return Ok(donations);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BloodDonationDto>> Create([FromBody] CreateBloodDonationDto dto)
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not logged in");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var created = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.DonationId }, created);
        }

        [HttpPost("sync")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BloodDonationDto>> CreateWithSync(CreateBloodDonationDto dto)
        {
            try
            {
                var created = await _service.CreateWithSynchronizedInfoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.DonationId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Thêm catch block có xử lý logger
                _logger.LogError(ex, "Lỗi khi đồng bộ thông tin hiến máu: {Message}", ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(Guid id, UpdateBloodDonationDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/status/{status}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            var result = await _service.UpdateStatusAsync(id, status);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("approve-blood-donation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ApproveBloodDonation([FromBody] ApproveBloodDonationDTO dto)
        {
            var result = await _service.ApproveDonationAsync(dto.DonationId, dto.ApproveDate);
            if (!result)
                return NotFound(new { message = "Donation not found" });
            return Ok(new { message = "Blood donation approved successfully." });
        }

        [HttpPost("reject-blood-donation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RejectBloodDonation([FromBody] RejectBloodDonationDTO dto)
        {
            var result = await _service.RejectDonationAsync(dto.DonationId, dto.RejectionReason, dto.RejectionDate);
            if (!result)
                return NotFound(new { message = "Donation not found" });
            return Ok(new { message = "Blood donation rejected successfully." });
        }

        [HttpPost("cancel-blood-donation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CancelBloodDonation([FromBody] CancelBloodDonationDTO dto)
        {
            var result = await _service.CancelDonationAsync(dto.DonationId, dto.CancelReason, dto.CancelDate);
            if (!result)
                return NotFound(new { message = "Donation not found" });
            return Ok(new { message = "Blood donation canceled successfully." });
        }

    }
}
