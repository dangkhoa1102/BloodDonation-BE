using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("blood-inventory")]
        public async Task<IActionResult> GetBloodInventoryStats()
        {
            var result = await _dashboardService.GetBloodInventoryStatsAsync();
            return Ok(result);
        }

        [HttpGet("donation-stats")]
        public async Task<IActionResult> GetDonationStats([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            var result = await _dashboardService.GetDonationStatsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("request-stats")]
        public async Task<IActionResult> GetRequestStats([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            var result = await _dashboardService.GetRequestStatsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("donor-stats")]
        public async Task<IActionResult> GetDonorStats()
        {
            var result = await _dashboardService.GetDonorStatsAsync();
            return Ok(result);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(result);
        }
    }
}