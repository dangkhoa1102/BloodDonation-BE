/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Repositories;
using Repositories.Interfaces;
using Models;


namespace Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IEnumerable<BlogResponseDto>> GetAllBlogsAsync()
        {
            var blogs = await _blogRepository.GetAllWithAuthorAsync();
            return blogs.Select(blog => new BlogResponseDto
            {
                BlogId = blog.BlogId,
                AuthorId = blog.AuthorId,
                Title = blog.Title,
                Content = blog.Content,
                PublishDate = blog.PublishDate,
                Category = blog.Category,
                ViewCount = blog.ViewCount
            });
        }

        public async Task<BlogResponseDto> GetBlogByIdAsync(Guid id)
        {
            var blog = await _blogRepository.GetByIdWithAuthorAsync(id);
            if (blog == null)
                return null;

            return new BlogResponseDto
            {
                BlogId = blog.BlogId,
                AuthorId = blog.AuthorId,
                Title = blog.Title,
                Content = blog.Content,
                PublishDate = blog.PublishDate,
                Category = blog.Category,
                ViewCount = blog.ViewCount
            };
        }

        public async Task<BlogResponseDto> CreateBlogAsync(BlogCreateDto blogCreateDto)
        {
            // Kiểm tra xem tiêu đề đã tồn tại chưa
            var titleExists = await _blogRepository.ExistsByTitleAsync(blogCreateDto.Title);
            if (titleExists)
                return null; // Có thể throw exception hoặc return error message

            var blog = new Blog
            {
                BlogId = Guid.NewGuid(),
                AuthorId = blogCreateDto.AuthorId,
                Title = blogCreateDto.Title,
                Content = blogCreateDto.Content,
                Category = blogCreateDto.Category,
                ViewCount = 0
            };

            await _blogRepository.AddAsync(blog);
            await _blogRepository.SaveChangesAsync();

            return new BlogResponseDto
            {
                BlogId = blog.BlogId,
                AuthorId = blog.AuthorId,
                Title = blog.Title,
                Content = blog.Content,
                PublishDate = blog.PublishDate,
                Category = blog.Category,
                ViewCount = blog.ViewCount
            };
        }

        public async Task<BlogResponseDto> UpdateBlogAsync(Guid id, BlogUpdateDto blogUpdateDto)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                return null;

            blog.Title = blogUpdateDto.Title;
            blog.Content = blogUpdateDto.Content;
            blog.Category = blogUpdateDto.Category;

            _blogRepository.Update(blog);
            await _blogRepository.SaveChangesAsync();

            return new BlogResponseDto
            {
                BlogId = blog.BlogId,
                AuthorId = blog.AuthorId,
                Title = blog.Title,
                Content = blog.Content,
                PublishDate = blog.PublishDate,
                Category = blog.Category,
                ViewCount = blog.ViewCount
            };
        }

        public async Task<bool> DeleteBlogAsync(Guid id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                return false;

            _blogRepository.Remove(blog);
            return await _blogRepository.SaveChangesAsync();
        }
    }
}
*/