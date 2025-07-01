using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BloodUnitRepository : BaseRepository<BloodUnit>, IBloodUnitRepository
    {
        public BloodUnitRepository(BloodDonationSupportContext context) : base(context)
        {
        }

        public async Task<BloodUnit> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .FirstOrDefaultAsync(b => b.UnitId == id);
        }

        public async Task<IEnumerable<BloodUnit>> GetAllWithDetailsAsync()
        {
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetByBloodTypeAsync(Guid bloodTypeId)
        {
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .Where(b => b.BloodTypeId == bloodTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetByComponentTypeAsync(Guid componentType)
        {
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .Where(b => b.ComponentType == componentType)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetByStatusAsync(string status)
        {
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetExpiredUnitsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.BloodUnits
                .Include(b => b.BloodType)
                .Include(b => b.ComponentTypeNavigation)
                .Include(b => b.Donation)
                .Where(b => b.ExpiryDate < today)
                .ToListAsync();
        }
    }
}