using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Enums;
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
        [HttpGet("quantities-by-type")]
        public async Task<IActionResult> GetQuantitiesByBloodType()
        {
            try
            {
                var quantities = await _bloodUnitService.GetQuantityByBloodTypeAsync();
                return Ok(quantities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood quantities by type");
                return StatusCode(500, "An error occurred while retrieving blood quantities");
            }
        }
        [HttpGet("total-quantity")]
        public async Task<IActionResult> GetTotalQuantity()
        {
            try
            {
                var totalQuantity = await _bloodUnitService.GetTotalBloodQuantityAsync();
                return Ok(totalQuantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total blood quantity");
                return StatusCode(500, "An error occurred while retrieving total blood quantity");
            }
        }
        [HttpPost("Blood-Unit-auto-update-expired")]
        [Authorize]
        public async Task<IActionResult> UpdateExpiredUnits()
        {
            try
            {
                await _bloodUnitService.UpdateExpiredUnitsAsync();
                return Ok(new { message = "Expired blood units have been updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expired blood units");
                return StatusCode(500, new { message = "An error occurred while updating expired blood units" });
            }
        }

        [HttpPut("Blood-Unit-mark-as-used/{unitId}")]
        [Authorize]
        public async Task<IActionResult> MarkAsUsed(Guid unitId, [FromQuery] Guid? requestId = null)
        {
            try
            {
                var (success, message) = await _bloodUnitService.MarkUnitAsUsedAsync(unitId, requestId);
                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking blood unit {UnitId} as used", unitId);
                return StatusCode(500, new { message = "An error occurred while marking blood unit as used" });
            }
        }

        [HttpPut("Blood-unit-update-status{unitId}")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(Guid unitId, [FromQuery] BloodUnitStatus newStatus)
        {
            try
            {
                var (success, message) = await _bloodUnitService.UpdateBloodUnitStatusAsync(unitId, newStatus);
                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blood unit {UnitId} status", unitId);
                return StatusCode(500, new { message = "An error occurred while updating blood unit status" });
            }
        }
    }
}