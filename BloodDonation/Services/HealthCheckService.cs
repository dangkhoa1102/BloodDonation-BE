using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Repositories.Interfaces;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _repo;
        private readonly IDonorRepository _donorRepo;
        private readonly BloodDonationSupportContext _context;

        public HealthCheckService(
            IHealthCheckRepository repo,
            IDonorRepository donorRepo,
            BloodDonationSupportContext context)
        {
            _repo = repo;
            _donorRepo = donorRepo;
            _context = context;
        }

        public async Task<IEnumerable<HealthCheck>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<HealthCheck> GetByIdAsync(Guid id)
            => await _repo.GetByIdAsync(id);

        public async Task<HealthCheck> AddAsync(HealthCheckDTO dto)
        {
            var donor = await _donorRepo.GetByUserIdCardAsync(dto.UserIdCard);
            if (donor == null)
                throw new ArgumentException("Không tìm thấy donor với userIdCard này");

            var entity = new HealthCheck
            {
                HealthCheckId = Guid.NewGuid(),
                DonorId = donor.DonorId,
                Weight = dto.Weight,
                Height = dto.Height,
                HeartRate = dto.HeartRate,
                Temperature = dto.Temperature,
                BloodPressure = dto.BloodPressure,
                MedicalHistory = dto.MedicalHistory,
                CurrentMedications = dto.CurrentMedications,
                Allergies = dto.Allergies,
                HealthCheckDate = dto.HealthCheckDate,
                HealthCheckStatus = dto.HealthCheckStatus
            };
            return await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(Guid id, HealthCheckDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Weight = dto.Weight;
                entity.Height = dto.Height;
                entity.HeartRate = dto.HeartRate;
                entity.Temperature = dto.Temperature;
                entity.BloodPressure = dto.BloodPressure;
                entity.MedicalHistory = dto.MedicalHistory;
                entity.CurrentMedications = dto.CurrentMedications;
                entity.Allergies = dto.Allergies;
                entity.HealthCheckDate = dto.HealthCheckDate;
                entity.HealthCheckStatus = dto.HealthCheckStatus;
                await _repo.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<List<Guid>> GetAvailableDonorIdsAsync()
            => await _repo.GetAvailableDonorIdsAsync();

        public async Task ApproveHealthCheckAsync(Guid healthCheckId)
        {
            var healthCheck = await _repo.GetByIdAsync(healthCheckId);
            if (healthCheck == null) throw new Exception("Không tìm thấy HealthCheck");

            // 2. Cập nhật trạng thái HealthCheck
            healthCheck.HealthCheckStatus = "Approved";
            await _repo.UpdateAsync(healthCheck);
            // Tìm Donor
            var donor = await _donorRepo.GetByIdAsync(healthCheck.DonorId);
            if (donor == null) throw new Exception("Không tìm thấy Donor");

            // Tìm BloodDonation đúng ngày HealthCheck
            var donation = await _context.BloodDonations
                .Where(d => d.DonorId == donor.DonorId && d.DonationDate == healthCheck.HealthCheckDate)
                .FirstOrDefaultAsync();

            if (donation == null) throw new Exception("Không tìm thấy BloodDonation phù hợp");

            // 1. Cập nhật trạng thái
            donation.Status = "Completed";
            _context.BloodDonations.Update(donation);

            // 2. Tạo DonationHistory mới
            var nextEligibleDate = donation.DonationDate!.Value.AddDays(84);
            var donationHistory = new DonationHistory
            {
                HistoryId = Guid.NewGuid(),
                DonorId = donation.DonorId,
                DonationDate = donation.DonationDate,
                Quantity = donation.Quantity ?? 0,
                HealthStatus = "Hiến máu thành công",
                NextEligibleDate = nextEligibleDate,
                CertificateId = donation.CertificateId
            };
            _context.DonationHistories.Add(donationHistory);

            // 3. Tạo BloodUnit mới
            var expiryDate = healthCheck.HealthCheckDate.AddDays(84); // 84 ngày cho máu toàn phần
            var bloodUnit = new BloodUnit
            {
                UnitId = Guid.NewGuid(),
                DonationId = donation.DonationId,
                BloodTypeId = donor.BloodTypeId,
                ComponentType = null, // Chưa biết, staff sẽ cập nhật sau
                Status = "available",
                ExpiryDate = expiryDate,
                Quantity = donation.Quantity ?? 0,
            };
            _context.BloodUnits.Add(bloodUnit);

            await _context.SaveChangesAsync();
        }

    }
}