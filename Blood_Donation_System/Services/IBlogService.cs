using Blood_Donation_System.DTOs.Blog;

namespace Blood_Donation_System.Services
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
