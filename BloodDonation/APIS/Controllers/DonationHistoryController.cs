using Microsoft.AspNetCore.Authorization;
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
        private readonly IDonationHistoryService _donationHistoryService;

        public DonationHistoryController(IDonationHistoryService donationHistoryService)
        {
            _donationHistoryService = donationHistoryService;
        }

        // GET: api/donationhistory
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var histories = await _donationHistoryService.GetAllAsync();
            return Ok(histories);
        }

        // GET: api/donationhistory/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var history = await _donationHistoryService.GetByIdAsync(id);
            if (history == null)
                return NotFound();
            return Ok(history);
        }

        // POST: api/donationhistory
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonationHistoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _donationHistoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.HistoryId }, result);
        }

        // PUT: api/donationhistory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonationHistoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _donationHistoryService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/donationhistory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _donationHistoryService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
