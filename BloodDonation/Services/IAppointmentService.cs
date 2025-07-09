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
        Task<AppointmentDetailDTO> CreateAppointmentAsync(CreateAppointmentDTO dto);
        Task<AppointmentDetailDTO> GetAppointmentByIdAsync(Guid donationId);
        Task<bool> UpdateDonationDateAsync(UpdateDonationDateDTO dto);
    }
}
