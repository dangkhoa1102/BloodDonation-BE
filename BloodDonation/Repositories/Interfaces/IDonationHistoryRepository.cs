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
        // Thêm phương thức đặc thù nếu cần, ví dụ:
        Task<IEnumerable<DonationHistory>> GetByDonorIdAsync(Guid donorId);
    }
}
