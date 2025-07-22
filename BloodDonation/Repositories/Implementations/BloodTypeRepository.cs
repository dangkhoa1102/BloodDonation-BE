using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories.Implementations
{
    public class BloodTypeRepository : IBloodTypeRepository
    {
        private readonly BloodDonationSupportContext _context;
        

        public BloodTypeRepository(BloodDonationSupportContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodType>> GetAllAsync()
        {
            return await _context.BloodTypes.ToListAsync();
        }

        public async Task<BloodType> GetByIdAsync(Guid id)
        {
            return await _context.BloodTypes.FindAsync(id);
        }

        public async Task<int> GetAvailableUnitsCountAsync(Guid bloodTypeId)
        {
            return await _context.BloodUnits
                .Where(u => u.BloodTypeId == bloodTypeId && u.Status == "available")
                .CountAsync();
        }


        public async Task<int> GetTotalUnitsCountAsync(Guid bloodTypeId)
        {
            return await _context.BloodUnits
                .Where(u => u.BloodTypeId == bloodTypeId)
                .CountAsync();
        }
        public async Task<int> GetAvailableUnitsQuantityAsync(Guid bloodTypeId)
        {
            return await _context.BloodUnits
                .Where(u => u.BloodTypeId == bloodTypeId && u.Status == "available")
                .SumAsync(u => u.Quantity);
        }       
    }
}