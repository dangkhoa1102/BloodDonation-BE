using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories.Interfaces
{
    public interface IDonorRepository : IGenericRepository<Donor>
    {
        // Bạn có thể thêm các method đặc thù cho Donor ở đây nếu cần, ví dụ:
        Task<IEnumerable<Donor>> GetAvailableDonorsAsync();
    }
}
