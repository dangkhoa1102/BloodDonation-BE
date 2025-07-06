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
        
        Task<IEnumerable<Donor>> GetAvailableDonorsAsync();
        Task<Donor> GetByIdWithDetailsAsync(Guid id);

        Task<Donor> GetByUserIdCardAsync(string userIdCard);
    }
}
