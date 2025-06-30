using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories.Interfaces
{
    public interface IDonationHistoryRepository : IGenericRepository<DonationHistory>
    {
        Task<IEnumerable<DonationHistory>> GetAllWithDonorAsync();
        Task<DonationHistory> GetByIdWithDonorAsync(Guid id);
    }
}
