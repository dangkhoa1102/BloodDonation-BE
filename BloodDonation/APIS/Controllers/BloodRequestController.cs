using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;
using System.Security.Claims;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")] // Chỉ cho phép Staff truy cập
    public class BloodRequestController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;
        private readonly ILogger<BloodRequestController> _logger;

        public BloodRequestController(
            IBloodRequestService bloodRequestService,
            ILogger<BloodRequestController> logger)
        {
            _bloodRequestService = bloodRequestService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterBloodRequest([FromBody] BloodRequestRegistrationDTO request)
        {
            try
            {
                _logger.LogInformation("User claims: {Claims}", 
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));

                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                _logger.LogInformation("User role: {Role}", role);

                if (!User.IsInRole("Staff"))
                {
                    return Unauthorized(new { message = "User is not authorized to perform this action" });
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var staffIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(staffIdStr))
                    return Unauthorized(new { message = "Staff ID not found" });

                if (!Guid.TryParse(staffIdStr, out Guid staffId))
                    return BadRequest(new { message = "Invalid Staff ID format" });

                var (success, message, requestId) = await _bloodRequestService.RegisterBloodRequestAsync(
                    request, staffId);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message, requestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetRequestById(Guid requestId)
        {
            try
            {
                var request = await _bloodRequestService.GetRequestByIdAsync(requestId);
                if (request == null)
                    return NotFound(new { message = "Blood request not found" });

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRequestById endpoint");
                return StatusCode(500, new { message = "An error occurred while retrieving the request" });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var requests = await _bloodRequestService.GetPendingRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPendingRequests endpoint");
                return StatusCode(500, new { message = "An error occurred while retrieving pending requests" });
            }
        }
    }
}