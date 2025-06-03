using Blood_Donation_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blood_Donation_System.Repositories
{
    public interface IDonorRepository : IGenericRepository<Donor>
    {
        Task<Donor> GetDonorByUserIdAsync(Guid userId);
        Task<IEnumerable<Donor>> GetAvailableDonorsAsync();
        Task<IEnumerable<Donor>> GetDonorsByBloodTypeAsync(Guid bloodTypeId);
    }
} 