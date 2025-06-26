using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories.Interfaces
{
    public interface IBloodDonationRepository : IGenericRepository<BloodDonation> 
    {
        Task<IEnumerable<BloodDonation>> GetAllWithDetailsAsync();
        Task<BloodDonation> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<BloodDonation>> GetByDonorIdAsync(Guid donorId);
        Task<IEnumerable<BloodDonation>> GetByStatusAsync(string status);
    }

}
