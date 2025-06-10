using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Models;

namespace Repositories.Implementations
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly BloodDonationSupportContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(BloodDonationSupportContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
