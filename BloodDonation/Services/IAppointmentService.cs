using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services
{
    public interface IAppointmentService
    {
        
        Task<bool> UpdateDonationDateAsync(UpdateDonationDateDTO dto);
        Task<BloodDonationProcessDTO> GetLatestDonationProcessByDonorIdAsync(Guid donorId);
    }
}
