using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;

namespace APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _service;
        public CertificateController(ICertificateService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCertificateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CertificateId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCertificateDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }

}
