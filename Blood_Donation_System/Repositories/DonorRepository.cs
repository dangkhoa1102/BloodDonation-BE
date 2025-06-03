using Blood_Donation_System.Data;
using Blood_Donation_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blood_Donation_System.Repositories
{
    public class DonorRepository : GenericRepository<Donor>, IDonorRepository
    {
        public DonorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Donor> GetDonorByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<Donor>> GetAvailableDonorsAsync()
        {
            return await _dbSet.Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<IEnumerable<Donor>> GetDonorsByBloodTypeAsync(Guid bloodTypeId)
        {
            return await _dbSet.Where(d => d.BloodTypeId == bloodTypeId).ToListAsync();
        }
    }
} 