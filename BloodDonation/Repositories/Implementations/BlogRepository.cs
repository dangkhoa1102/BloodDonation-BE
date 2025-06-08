using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BlogRepository : GenericRepository<Blog>, IBlogRepository
    {
        public BlogRepository(BloodDonationSupportContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Blog>> GetAllWithAuthorAsync()
        {
            return await _dbSet.Include(b => b.Author).ToListAsync();
        }


        public async Task<Blog> GetByIdWithAuthorAsync(Guid id)
        {
            return await _dbSet.Include(b => b.Author)
                              .FirstOrDefaultAsync(b => b.BlogId == id);
        }


        public async Task<IEnumerable<Blog>> GetBlogsByCategoryAsync(string category)
        {
            return await _dbSet.Where(b => b.Category.ToLower() == category.ToLower())
                              .ToListAsync();
        }

        // Lấy blog theo tác giả
        public async Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(Guid authorId)
        {
            return await _dbSet.Where(b => b.AuthorId == authorId)
                              .ToListAsync();
        }

        // Tìm kiếm blog theo tiêu đề
        public async Task<IEnumerable<Blog>> SearchBlogsByTitleAsync(string title)
        {
            return await _dbSet.Where(b => b.Title.ToLower().Contains(title.ToLower()))
                              .ToListAsync();
        }

        // Kiểm tra xem tiêu đề đã tồn tại chưa
        public async Task<bool> ExistsByTitleAsync(string title)
        {
            return await _dbSet.AnyAsync(b => b.Title.ToLower() == title.ToLower());
        }

        // Đếm tổng số blog
        public async Task<int> GetTotalBlogsCountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
