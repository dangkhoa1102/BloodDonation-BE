using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodDonationController : ControllerBase
    {
        private readonly IBloodDonationService _service;

        public BloodDonationController(IBloodDonationService service)
        {
            _service = service;
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

        [HttpPost]
        public async Task<ActionResult<BloodDonationDto>> Create(CreateBloodDonationDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DonationId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateBloodDonationDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
