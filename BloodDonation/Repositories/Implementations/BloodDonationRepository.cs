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
    public class BloodDonationRepository : GenericRepository<BloodDonation>, IBloodDonationRepository
    {
        public BloodDonationRepository(BloodDonationSupportContext context) : base(context) { }

        public async Task<IEnumerable<BloodDonation>> GetAllWithDetailsAsync()
        {
            // Chỉ lấy các trường thực sự có trong cơ sở dữ liệu, và include các quan hệ
            return await _dbSet
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.User)
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.BloodType)
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.Location)
                .Include(d => d.Request)
                .Include(d => d.Certificate)
                .ToListAsync();
        }

        public async Task<BloodDonation> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.User)
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.BloodType)
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.Location)
                .Include(d => d.Request)
                .Include(d => d.Certificate)
                .FirstOrDefaultAsync(d => d.DonationId == id);
        }

        public async Task<IEnumerable<BloodDonation>> GetByDonorIdAsync(Guid donorId)
        {
            return await _dbSet
                .Include(d => d.Donor)
                .Include(d => d.Request)
                .Include(d => d.Certificate)
                .Where(d => d.DonorId == donorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodDonation>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(d => d.Donor)
                .Include(d => d.Request)
                .Where(d => d.Status == status)
                .ToListAsync();
        }
    }
}


