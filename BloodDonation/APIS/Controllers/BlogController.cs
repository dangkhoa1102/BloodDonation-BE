using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: api/blog
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return Ok(blogs);
        }

        // GET: api/blog/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
                return NotFound();

            return Ok(blog);
        }

        // POST: api/blog
        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromBody] BlogCreateDto blogCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var blog = await _blogService.CreateBlogAsync(blogCreateDto);
            return CreatedAtAction(nameof(GetBlogById), new { id = blog.BlogId }, blog);
        }

        // PUT: api/blog/{id}
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogUpdateDto blogUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var blog = await _blogService.UpdateBlogAsync(id, blogUpdateDto);
            if (blog == null)
                return NotFound();

            return Ok(blog);
        }

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var result = await _blogService.DeleteBlogAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
