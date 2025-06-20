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
        private readonly BloodDonationSupportContext _context;

        public DonationHistoryRepository(BloodDonationSupportContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationHistory>> GetByDonorIdAsync(Guid donorId)
        {
            return await _context.Set<DonationHistory>()
                .Where(d => d.DonorId == donorId)
                .ToListAsync();
        }
    }
}
