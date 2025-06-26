using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services
{
    public interface IBloodDonationService
    {
        Task<IEnumerable<BloodDonationDto>> GetAllAsync();
        Task<BloodDonationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<BloodDonationDto>> GetByDonorIdAsync(Guid donorId);
        Task<BloodDonationDto> CreateAsync(CreateBloodDonationDto dto);
        Task<BloodDonationDto> CreateWithSynchronizedInfoAsync(CreateBloodDonationDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateBloodDonationDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, string status);
    }


}
