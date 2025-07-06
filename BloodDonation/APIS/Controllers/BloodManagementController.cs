using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BloodManagementController : ControllerBase
    {
        private readonly IBloodManagementService _bloodManagementService;
        private readonly ILogger<BloodManagementController> _logger;

        public BloodManagementController(
            IBloodManagementService bloodManagementService,
            ILogger<BloodManagementController> logger)
        {
            _bloodManagementService = bloodManagementService;
            _logger = logger;
        }

        [HttpGet("Get-Blood-types")]
        public async Task<IActionResult> GetBloodTypes()
        {
            try
            {
                var bloodTypes = await _bloodManagementService.GetAllBloodTypesAsync();
                if (!bloodTypes.Any())
                {
                    return NoContent();
                }
                return Ok(bloodTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood types");
                return StatusCode(500, new { message = "An error occurred while retrieving blood types" });
            }
        }

        [HttpGet("Get-blood-types/{id}")]
        public async Task<IActionResult> GetBloodTypeById(Guid id)
        {
            try
            {
                var bloodType = await _bloodManagementService.GetBloodTypeByIdAsync(id);
                if (bloodType == null)
                {
                    return NotFound(new { message = "Blood type not found" });
                }
                return Ok(bloodType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood type with ID: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the blood type" });
            }
        }

        [HttpGet("Get-blood-components")]
        public async Task<IActionResult> GetBloodComponents()
        {
            try
            {
                var components = await _bloodManagementService.GetAllBloodComponentsAsync();
                if (!components.Any())
                {
                    return NoContent();
                }
                return Ok(components);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood components");
                return StatusCode(500, new { message = "An error occurred while retrieving blood components" });
            }
        }

        [HttpGet("Get-blood-components/{id}")]
        public async Task<IActionResult> GetBloodComponentById(Guid id)
        {
            try
            {
                var component = await _bloodManagementService.GetBloodComponentByIdAsync(id);
                if (component == null)
                {
                    return NotFound(new { message = "Blood component not found" });
                }
                return Ok(component);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood component with ID: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the blood component" });
            }
        }
        [Route("api/[controller]")]
        [ApiController]
        public class BloodCompatibilityController : ControllerBase
        {
            private readonly IBloodManagementService _bloodManagementService;
            private readonly ILogger<BloodCompatibilityController> _logger;

            public BloodCompatibilityController(
                IBloodManagementService bloodManagementService,
                ILogger<BloodCompatibilityController> logger)
            {
                _bloodManagementService = bloodManagementService;
                _logger = logger;
            }         
        }
        [HttpGet("Get-blood-type-Compatibility/{bloodType}")]
        public async Task<IActionResult> GetBloodTypeCompatibility(string bloodType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bloodType))
                {
                    return BadRequest(new { message = "Blood type is required" });
                }

                var compatibility = await _bloodManagementService.GetBloodTypeCompatibilityAsync(bloodType);
                return Ok(compatibility);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blood type compatibility for {BloodType}", bloodType);
                return StatusCode(500, new { message = "An error occurred while retrieving blood type compatibility" });
            }
        }

        [HttpGet("Get-component-Compatibility/{componentName}")]
        public async Task<IActionResult> GetComponentCompatibility(string componentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(componentName))
                {
                    return BadRequest(new { message = "Component name is required" });
                }

                var compatibility = await _bloodManagementService.GetComponentCompatibilityAsync(componentName);
                return Ok(compatibility);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting component compatibility for {ComponentName}", componentName);
                return StatusCode(500, new { message = "An error occurred while retrieving component compatibility" });
            }
        }
    }
}