using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using static Models.DTOs.DonorDTO;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public DonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonorDto>>> GetAll()
        {
            var donors = await _donorService.GetAllAsync();
            return Ok(donors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DonorDto>> GetById(Guid id)
        {
            var donor = await _donorService.GetByIdAsync(id);
            if (donor == null)
                return NotFound();

            return Ok(donor);
        }

        [HttpPost]
        public async Task<ActionResult<DonorDto>> Create([FromBody] CreateDonorDto dto)
        {
            var created = await _donorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DonorId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDonorDto dto)
        {
            var success = await _donorService.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _donorService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }


    }
}
