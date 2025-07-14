using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Interfaces;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BloodUnitController : ControllerBase
    {
        private readonly IBloodUnitService _bloodUnitService;
        private readonly ILogger<BloodUnitController> _logger;


        public BloodUnitController(
            IBloodUnitService bloodUnitService,
            ILogger<BloodUnitController> logger)
        {
            _bloodUnitService = bloodUnitService;
            _logger = logger;
        }

        [HttpGet("Get-All-BloodUnit")]
        public async Task<IActionResult> GetAllBloodUnits()
        {
            try
            {
                var units = await _bloodUnitService.GetAllBloodUnitsAsync();
                if (!units.Any())
                    return NoContent();

                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units");
                return StatusCode(500, new { message = "An error occurred while retrieving blood units" });
            }
        }

        [HttpGet("Get-BloodUnit-By-id/{id}")]
        public async Task<IActionResult> GetBloodUnitById(Guid id)
        {
            try
            {
                var unit = await _bloodUnitService.GetBloodUnitByIdAsync(id);
                if (unit == null)
                    return NotFound(new { message = "Blood unit not found" });

                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood unit");
                return StatusCode(500, new { message = "An error occurred while retrieving the blood unit" });
            }
        }

        [HttpGet("Get-BloodUnit-by-blood-type/{bloodTypeId}")]
        public async Task<IActionResult> GetByBloodType(Guid bloodTypeId)
        {
            try
            {
                var units = await _bloodUnitService.GetBloodUnitsByTypeAsync(bloodTypeId);
                if (!units.Any())
                    return NoContent();

                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by blood type");
                return StatusCode(500, new { message = "An error occurred while retrieving blood units" });
            }
        }

        [HttpGet("Get-BloodUnit-by-component/{componentId}")]
        public async Task<IActionResult> GetByComponent(Guid componentId)
        {
            try
            {
                var units = await _bloodUnitService.GetBloodUnitsByComponentAsync(componentId);
                if (!units.Any())
                    return NoContent();

                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by component");
                return StatusCode(500, new { message = "An error occurred while retrieving blood units" });
            }
        }

        [HttpGet("Get-BloodUnit-by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var units = await _bloodUnitService.GetBloodUnitsByStatusAsync(status);
                if (!units.Any())
                    return NoContent();

                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by status");
                return StatusCode(500, new { message = "An error occurred while retrieving blood units" });
            }
        }

        [HttpGet("Get-BloodUnit-expired")]
        public async Task<IActionResult> GetExpiredUnits()
        {
            try
            {
                var units = await _bloodUnitService.GetExpiredBloodUnitsAsync();
                if (!units.Any())
                    return NoContent();

                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expired blood units");
                return StatusCode(500, new { message = "An error occurred while retrieving expired blood units" });
            }
        }
        [HttpPatch("Update-Blood-Unit/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBloodUnit(Guid id, [FromBody] UpdateBloodUnitDTO dto)
        {
            try
            {
                var (success, message) = await _bloodUnitService.UpdateBloodUnitAsync(id, dto);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blood unit {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the blood unit" });
            }
        }
    }
}