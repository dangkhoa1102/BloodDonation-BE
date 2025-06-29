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
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly BloodDonationSupportContext _context;
        public HealthCheckRepository(BloodDonationSupportContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HealthCheck>> GetAllAsync()
            => await _context.HealthChecks.ToListAsync();

        public async Task<HealthCheck> GetByIdAsync(Guid id)
        {
            return await _context.HealthChecks.FindAsync(id);
        }

        public async Task<HealthCheck> AddAsync(HealthCheck healthCheck)
        {
            await _context.HealthChecks.AddAsync(healthCheck);
            await _context.SaveChangesAsync();
            return healthCheck;
        }

        public async Task UpdateAsync(HealthCheck healthCheck)
        {
            _context.HealthChecks.Update(healthCheck);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.HealthChecks.FindAsync(id);
            if (entity != null)
            {
                _context.HealthChecks.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Guid>> GetAvailableDonorIdsAsync()
            => await _context.Donors.Select(d => d.DonorId).ToListAsync();
    }
}
