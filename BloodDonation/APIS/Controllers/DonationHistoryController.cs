using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services;

namespace APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationHistoryController : ControllerBase
    {
        private readonly IDonationHistoryService _service;

        public DonationHistoryController(IDonationHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DonationHistory model)
        {
            var result = await _service.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.HistoryId }, result);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] DonationHistoryCreateDto dto)
        //{
        //    // Xử lý tạo mới
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
