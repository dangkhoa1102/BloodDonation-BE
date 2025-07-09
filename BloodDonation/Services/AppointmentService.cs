using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly BloodDonationSupportContext _context;

        public AppointmentService(BloodDonationSupportContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDetailDTO> CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            var appointment = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = dto.DonorId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                Status = "Scheduled",
                Notes = dto.Notes
            };
            _context.BloodDonations.Add(appointment);
            await _context.SaveChangesAsync();

            return new AppointmentDetailDTO
            {
                DonationId = appointment.DonationId,
                DonorId = appointment.DonorId,
                DonationDate = appointment.DonationDate,
                Quantity = appointment.Quantity,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        public async Task<AppointmentDetailDTO> GetAppointmentByIdAsync(Guid donationId)
        {
            var appointment = await _context.BloodDonations.FindAsync(donationId);
            if (appointment == null) return null;

            return new AppointmentDetailDTO
            {
                DonationId = appointment.DonationId,
                DonorId = appointment.DonorId,
                DonationDate = appointment.DonationDate,
                Quantity = appointment.Quantity,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        public async Task<bool> UpdateDonationDateAsync(UpdateDonationDateDTO dto)
        {
            var donation = await _context.BloodDonations.FindAsync(dto.DonationId);
            if (donation == null) return false;

            donation.DonationDate = dto.DonationDate;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
