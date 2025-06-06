using Blood_Donation_System.Models;

namespace Blood_Donation_System.Repositories
{
    public interface IBlogRepository : IGenericRepository<Blog>
    {
        Task<IEnumerable<Blog>> GetAllWithAuthorAsync();
        Task<Blog> GetByIdWithAuthorAsync(Guid id);
        Task<IEnumerable<Blog>> GetBlogsByCategoryAsync(string category);
        Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(Guid authorId);
        Task<IEnumerable<Blog>> SearchBlogsByTitleAsync(string title);
        Task<bool> ExistsByTitleAsync(string title);
        Task<int> GetTotalBlogsCountAsync();
    }
}
