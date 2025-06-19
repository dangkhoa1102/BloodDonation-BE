using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BloodComponentRepository : IBloodComponentRepository
    {
        private readonly BloodDonationSupportContext _context;

        public BloodComponentRepository(BloodDonationSupportContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodComponent>> GetAllAsync()
        {
            return await _context.BloodComponents.ToListAsync();
        }

        public async Task<BloodComponent> GetByIdAsync(Guid id)
        {
            return await _context.BloodComponents.FindAsync(id);
        }

        public async Task<int> GetAvailableUnitsCountAsync(Guid componentId)
        {
            return await _context.BloodUnits
                .Where(u => u.ComponentType == componentId && u.Status == "Available")
                .CountAsync();
        }

        public async Task<int> GetTotalUnitsCountAsync(Guid componentId)
        {
            return await _context.BloodUnits
                .Where(u => u.ComponentType == componentId)
                .CountAsync();
        }
        public async Task<BloodComponent> GetByNameAsync(string componentName)
        {
            return await _context.BloodComponents
                .FirstOrDefaultAsync(bc => bc.ComponentName == componentName);
        }
    }
}