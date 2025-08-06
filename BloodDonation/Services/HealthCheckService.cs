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
            
            var approvedHealthCheck = await _context.HealthChecks
        .Where(h => h.DonorId == donor.DonorId && h.HealthCheckStatus == "Approved")
        .OrderByDescending(h => h.HealthCheckDate)
        .FirstOrDefaultAsync();

            if (approvedHealthCheck != null)
            {
                // Kiểm tra xem đã có BloodDonation sử dụng HealthCheck này chưa
                var used = await _context.BloodDonations
                    .AnyAsync(d => d.DonorId == donor.DonorId && d.DonationDate == approvedHealthCheck.HealthCheckDate);

                if (!used)
                {
                    // Nếu chưa dùng, không cho tạo mới
                    throw new InvalidOperationException("Donor đã có HealthCheck Approved chưa sử dụng cho lần hiến máu nào.");
                }
            }

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

            //// Tìm BloodDonation theo DonorId và HealthCheckDate
            var bloodDonation = await _context.BloodDonations
        .FirstOrDefaultAsync(b => b.DonorId == donor.DonorId && b.DonationDate == dto.HealthCheckDate);

            // Nếu tìm thấy và DTO có Quantity thì cập nhật
            if (bloodDonation != null && dto.Quantity.HasValue)
            {
                bloodDonation.Quantity = dto.Quantity.Value;
                _context.BloodDonations.Update(bloodDonation);
                await _context.SaveChangesAsync();
            }

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

        public async Task ApproveHealthCheckAsync(Guid healthCheckId, Guid staffId)
        {
            var healthCheck = await _repo.GetByIdAsync(healthCheckId);
            if (healthCheck == null) throw new Exception("Không tìm thấy HealthCheck");

            // Tìm Donor
            var donor = await _donorRepo.GetByIdAsync(healthCheck.DonorId);
            if (donor == null) throw new Exception("Không tìm thấy Donor");

            // Tìm BloodDonation đúng ngày HealthCheck
            // chỉ kiểm tra ngày, không ktra giờ phút giây
            var donation = await _context.BloodDonations
    .Where(d => d.DonorId == donor.DonorId
        && d.DonationDate.HasValue
        && d.DonationDate.Value.Date == healthCheck.HealthCheckDate.Date)
    .OrderByDescending(d => d.DonationDate)
    .FirstOrDefaultAsync();

            if (donation == null) throw new Exception("Không tìm thấy BloodDonation phù hợp");

            // Cập nhật trạng thái các HealthCheck "Approved" cũ thành "Used" nếu đã có BloodDonation
            var oldApprovedChecks = await _context.HealthChecks
                .Where(h => h.DonorId == donor.DonorId && h.HealthCheckStatus == "Approved" && h.HealthCheckId != healthCheckId)
                .ToListAsync();

            foreach (var oldCheck in oldApprovedChecks)
            {
                var used = await _context.BloodDonations.AnyAsync(d =>
                    d.DonorId == donor.DonorId && d.DonationDate == oldCheck.HealthCheckDate);
                if (used)
                {
                    oldCheck.HealthCheckStatus = "Used";
                    _context.HealthChecks.Update(oldCheck);
                }
            }

            // 1. Cập nhật trạng thái HealthCheck hiện tại
            healthCheck.HealthCheckStatus = "Approved";
            await _repo.UpdateAsync(healthCheck);

            // 2. Cập nhật trạng thái BloodDonation
            donation.Status = "Completed";
            _context.BloodDonations.Update(donation);

            // --- Tạo Certificate mới ---
            var now = DateTime.Now;
            var certificate = new Certificate
            {
                CertificateId = Guid.NewGuid(),
                DonorId = donor.DonorId,
                DonationId = donation.DonationId,
                StaffId = staffId,
                CertificateType = "Blood Donation", // hoặc loại phù hợp
                CertificateNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(), // hoặc logic sinh số certificate
                IssueDate = DateOnly.FromDateTime(now),
                CreatedDate = now,
                LastModified = now
            };
            _context.Add(certificate);

            // Gán CertificateId cho BloodDonation và DonationHistory
            donation.CertificateId = certificate.CertificateId;


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
                ExpiryDate = DateOnly.FromDateTime(expiryDate),
                Quantity = donation.Quantity ?? 0,
            };
            _context.BloodUnits.Add(bloodUnit);

            await _context.SaveChangesAsync();
        }

        public async Task RejectHealthCheckAsync(Guid healthCheckId, Guid staffId)
        {
            var healthCheck = await _repo.GetByIdAsync(healthCheckId);
            if (healthCheck == null) throw new Exception("Không tìm thấy HealthCheck");

            // Tìm Donor
            var donor = await _donorRepo.GetByIdAsync(healthCheck.DonorId);
            if (donor == null) throw new Exception("Không tìm thấy Donor");

            // Tìm BloodDonation đúng ngày HealthCheck
            var donation = await _context.BloodDonations
                .Where(d => d.DonorId == donor.DonorId
                    && d.DonationDate.HasValue
                    && d.DonationDate.Value.Date == healthCheck.HealthCheckDate.Date)
                .OrderByDescending(d => d.DonationDate)
                .FirstOrDefaultAsync();

            // Cập nhật trạng thái các HealthCheck "Approved" cũ thành "Used" nếu đã có BloodDonation
            var oldApprovedChecks = await _context.HealthChecks
                .Where(h => h.DonorId == donor.DonorId && h.HealthCheckStatus == "Approved" && h.HealthCheckId != healthCheckId)
                .ToListAsync();

            foreach (var oldCheck in oldApprovedChecks)
            {
                var used = await _context.BloodDonations.AnyAsync(d =>
                    d.DonorId == donor.DonorId && d.DonationDate == oldCheck.HealthCheckDate);
                if (used)
                {
                    oldCheck.HealthCheckStatus = "Used";
                    _context.HealthChecks.Update(oldCheck);
                }
            }

            // 1. Cập nhật trạng thái HealthCheck hiện tại
            healthCheck.HealthCheckStatus = "Rejected";
            await _repo.UpdateAsync(healthCheck);

            // 2. Nếu có BloodDonation, cập nhật trạng thái
            if (donation != null)
            {
                donation.Status = "Rejected";
                _context.BloodDonations.Update(donation);
            }

            await _context.SaveChangesAsync();
        }

    }
}