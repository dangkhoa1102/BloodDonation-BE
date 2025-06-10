using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories.Interfaces
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
