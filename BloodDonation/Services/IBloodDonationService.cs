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
        Task<BloodDonationDto> CreateAsync(CreateBloodDonationDto dto, Guid userId);
        Task<BloodDonationDto> CreateWithSynchronizedInfoAsync(CreateBloodDonationDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateBloodDonationDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, string status);

        Task<bool> ApproveDonationAsync(Guid donationId, DateOnly? approveDate);
        Task<bool> RejectDonationAsync(Guid donationId, string reason, DateOnly? rejectionDate);
        Task<bool> CancelDonationAsync(Guid donationId, string reason, DateOnly? cancelDate);


    }


}
