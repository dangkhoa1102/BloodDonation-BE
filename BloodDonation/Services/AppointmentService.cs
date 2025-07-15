using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly BloodDonationSupportContext _context;

        public AppointmentService(BloodDonationSupportContext context)
        {
            _context = context;
        }


        public async Task<BloodDonationProcessDTO> GetLatestDonationProcessByDonorIdAsync(Guid donorId)
        {
            var donation = await _context.BloodDonations
                .Where(d => d.DonorId == donorId)
                .OrderByDescending(d => d.DonationDate)
                .FirstOrDefaultAsync();

            if (donation == null) return null;

            var healthCheck = await _context.HealthChecks
                .Where(hc => hc.Donor.DonorId == donorId && hc.HealthCheckDate <= donation.DonationDate)
                .OrderByDescending(hc => hc.HealthCheckDate)
                .FirstOrDefaultAsync();

            return new BloodDonationProcessDTO
            {
                DonationId = donation.DonationId,
                Status = donation.Status,
                HealthCheckDate = healthCheck?.HealthCheckDate,
                HealthCheckStatus = healthCheck?.HealthCheckStatus,
                CertificateId = donation.CertificateId,
                Notes = donation.Notes,
                DonorId = donation.DonorId
            };
        }

        public async Task<bool> UpdateDonationDateAsync(UpdateDonationDateDTO dto)
        {
            var donation = await _context.BloodDonations.FindAsync(dto.DonationId);
            if (donation == null) return false;

            donation.DonationDate = dto.DonationDate.ToDateTime(TimeOnly.MinValue);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
