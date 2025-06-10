using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogResponseDto>> GetAllBlogsAsync();
        Task<BlogResponseDto> GetBlogByIdAsync(Guid id);
        Task<BlogResponseDto> CreateBlogAsync(BlogCreateDto blogCreateDto);
        Task<BlogResponseDto> UpdateBlogAsync(Guid id, BlogUpdateDto blogUpdateDto);
        Task<bool> DeleteBlogAsync(Guid id);
    }
}
