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
    public class DonationHistoryRepository : GenericRepository<DonationHistory>, IDonationHistoryRepository
    {
        public DonationHistoryRepository(BloodDonationSupportContext context) : base(context) { }

        public async Task<IEnumerable<DonationHistory>> GetAllWithDonorAsync()
        {
            return await _dbSet.Include(d => d.Donor).ToListAsync();
        }

        public async Task<DonationHistory> GetByIdWithDonorAsync(Guid id)
        {
            return await _dbSet.Include(d => d.Donor)
                               .FirstOrDefaultAsync(d => d.HistoryId == id);
        }
    }
}
