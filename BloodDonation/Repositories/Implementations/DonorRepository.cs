using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class DonorRepository : GenericRepository<Donor>, IDonorRepository
    {
        public DonorRepository(BloodDonationSupportContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Donor>> GetAvailableDonorsAsync()
        {
            return await _dbSet.Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<Donor> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.BloodType)
                .Include(d => d.Location)
                .FirstOrDefaultAsync(d => d.DonorId == id);
        }
    }
}
