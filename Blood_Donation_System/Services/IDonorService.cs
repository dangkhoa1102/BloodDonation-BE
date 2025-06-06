 using Blood_Donation_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public interface IDonorService
    {
        Task<IEnumerable<Donor>> GetAllDonorsAsync();
        Task<Donor> GetDonorByIdAsync(Guid id);
        Task<Donor> GetDonorByUserIdAsync(Guid userId);
        Task<IEnumerable<Donor>> GetAvailableDonorsAsync();
        Task<IEnumerable<Donor>> GetDonorsByBloodTypeAsync(Guid bloodTypeId);
        Task<bool> AddDonorAsync(Donor donor);
        Task<bool> UpdateDonorAsync(Donor donor);
        Task<bool> DeleteDonorAsync(Guid id);
    }
} 