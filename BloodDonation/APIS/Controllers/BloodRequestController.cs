using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Models.Enums;
using Services;
using System.Security.Claims;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("Get-All-Request")]
        [Authorize]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var requests = await _bloodRequestService.GetAllRequestsAsync();
                if (!requests.Any())
                {
                    return NoContent();
                }

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood requests");
                return StatusCode(500, new { message = "An error occurred while retrieving blood requests" });
            }
        }

        [HttpGet("Get-Request-By-Id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            try
            {
                var request = await _bloodRequestService.GetRequestByIdAsync(id);
                if (request == null)
                {
                    return NotFound(new { message = "Blood request not found" });
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood request with ID: {RequestId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the blood request" });
            }
        }

        [HttpGet("Get-Request-By-status/{status}")]
        [Authorize]
        public async Task<IActionResult> GetRequestsByStatus([FromRoute] BloodRequestStatus status)
        {
            try
            {
                var requests = await _bloodRequestService.GetRequestsByStatusAsync(status);
                if (!requests.Any())
                {
                    return NoContent();
                }

                return Ok(new
                {
                    status = status.ToString(),
                    totalRequests = requests.Count(),
                    requests = requests
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood requests with status: {Status}", status);
                return StatusCode(500, new { message = "An error occurred while retrieving blood requests" });
            }
        }

        [HttpGet("Get-Request-By-Recipient-Name/{recipientName}")]
        [Authorize]
        public async Task<IActionResult> GetRequestsByRecipientName([FromRoute] string recipientName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recipientName))
                {
                    return BadRequest(new { message = "Recipient name cannot be empty" });
                }

                var requests = await _bloodRequestService.GetRequestsByRecipientNameAsync(recipientName);
                if (!requests.Any())
                {
                    return NoContent();
                }

                return Ok(new
                {
                    recipientName = recipientName,
                    totalRequests = requests.Count(),
                    requests = requests.Select(r => new
                    {
                        r.RequestId,
                        r.RecipientId,
                        r.BloodTypeRequired,
                        r.QuantityNeeded,
                        r.UrgencyLevel,
                        r.RequestDate,
                        r.Status,
                        r.Description
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood requests for recipient: {RecipientName}", recipientName);
                return StatusCode(500, new { message = "An error occurred while retrieving blood requests" });
            }
        }
        [HttpGet("Get-My-Requests")]
        [Authorize]
        public async Task<IActionResult> GetMyRequests()
        {
            try
            {
                // Lấy userId từ token
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    return BadRequest(new { message = "Invalid user identification" });
                }

                var requests = await _bloodRequestService.GetRequestsByRecipientUserIdAsync(userId);
                if (!requests.Any())
                {
                    return NoContent();
                }

                // Tạo response chi tiết
                var response = new
                {
                    totalRequests = requests.Count(),
                    requests = requests.Select(r => new
                    {
                        r.RequestId,
                        r.BloodTypeRequired,
                        r.QuantityNeeded,
                        r.UrgencyLevel,
                        RequestDate = r.RequestDate?.ToString("yyyy-MM-dd"),
                        r.Status,
                        r.Description,
                        bloodDonations = r.BloodDonations.Select(d => new
                        {
                            d.DonationId,
                            d.DonationDate,
                            d.Quantity,
                            d.Status
                        }).ToList()
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood requests for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving your blood requests" });
            }
        }

        [HttpGet("Get-My-Requests-By-Status/{status}")]
        [Authorize]
        public async Task<IActionResult> GetMyRequestsByStatus([FromRoute] BloodRequestStatus status)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                {
                    return BadRequest(new { message = "Invalid user identification" });
                }

                var allRequests = await _bloodRequestService.GetRequestsByRecipientUserIdAsync(userId);
                var filteredRequests = allRequests.Where(r => r.Status == status.ToString());

                if (!filteredRequests.Any())
                {
                    return NoContent();
                }

                var response = new
                {
                    status = status.ToString(),
                    totalRequests = filteredRequests.Count(),
                    requests = filteredRequests.Select(r => new
                    {
                        r.RequestId,
                        r.BloodTypeRequired,
                        r.QuantityNeeded,
                        r.UrgencyLevel,
                        RequestDate = r.RequestDate?.ToString("yyyy-MM-dd"),
                        r.Status,
                        r.Description,
                        bloodDonations = r.BloodDonations.Select(d => new
                        {
                            d.DonationId,
                            d.DonationDate,
                            d.Quantity,
                            d.Status
                        }).ToList()
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood requests by status for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving your blood requests" });
            }
        }
        [HttpPut("Update-Blood-Requests/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBloodRequest(Guid id, [FromBody] BloodRequestUpdateDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new InvalidOperationException("Staff ID not found in token"));

                var (success, message) = await _bloodRequestService.UpdateBloodRequestAsync(id, updateDto, staffId);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blood request");
                return StatusCode(500, new { message = "An error occurred while updating the blood request" });
            }
        }
        [HttpPost("reject-blood-request")]
        [Authorize]
        public async Task<IActionResult> RejectBloodRequest([FromBody] BloodRequestRejectDTO rejectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
              
                var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new InvalidOperationException("Staff ID not found in token"));

                var (success, message) = await _bloodRequestService.RejectBloodRequestAsync(rejectDto.RequestId,rejectDto, staffId);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting blood request {RequestId}", rejectDto.RequestId);
                return StatusCode(500, new { message = "An error occurred while rejecting the blood request" });
            }
        }

        [HttpPost("approve-blood-request")]
        [Authorize]
        public async Task<IActionResult> ApproveBloodRequest(Guid requestId)
        {
            try
            {
                var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new InvalidOperationException("Staff ID not found in token"));

                var (success, message) = await _bloodRequestService.ApproveBloodRequestAsync(requestId, staffId);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving blood request {RequestId}", requestId);
                return StatusCode(500, new { message = "An error occurred while approving the blood request" });
            }
        }
        [HttpPost("register-emergency")]
        [Authorize]
        public async Task<IActionResult> RegisterEmergencyRequest([FromBody] EmergencyBloodRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", errors));
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new InvalidOperationException("Staff ID not found in token"));

                var (success, message, requestId) = await _bloodRequestService.RegisterEmergencyBloodRequestAsync(
                    request, staffId);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new
                {
                    success = true,
                    message,
                    data = new
                    {
                        requestId,
                        requestDetails = new
                        {
                            patientName = request.PatientName,
                            email = request.Email,
                            userIdCard = request.UserIdCard,
                            bloodType = request.BloodTypeRequired,
                            quantityNeeded = request.QuantityNeeded,
                            contactPhone = request.Phone,
                            dateOfBirth = request.DateOfBirth?.ToString("yyyy-MM-dd"),
                            description = request.Description,
                            status = "Pending",
                            urgencyLevel = "Emergency",
                            requestDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing emergency blood request");
                return StatusCode(500, new { message = "An error occurred while processing the emergency request" });
            }
        }
    }
}