using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IDonationHistoryService
    {
        Task<IEnumerable<DonationHistory>> GetAllAsync();
        Task<DonationHistory?> GetByIdAsync(Guid id);
        Task<DonationHistory> AddAsync(DonationHistory history);
        Task<bool> DeleteAsync(Guid id);
    }
}
