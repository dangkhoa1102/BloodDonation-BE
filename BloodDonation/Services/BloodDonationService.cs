using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models;
using AutoMapper;
using Repositories.Interfaces;
using Microsoft.Extensions.Logging;


namespace Services
{
    public class BloodDonationService : IBloodDonationService
    {
        private readonly IBloodDonationRepository _donationRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Location> _locationRepository;
        private readonly IGenericRepository<BloodType> _bloodTypeRepository;
        private readonly ILogger<BloodDonationService> _logger;
        private readonly BloodDonationSupportContext _context;

        public BloodDonationService(
            IBloodDonationRepository donationRepository,
            IDonorRepository donorRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<Location> locationRepository,
            IGenericRepository<BloodType> bloodTypeRepository,
            ILogger<BloodDonationService> logger,
            BloodDonationSupportContext context)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _bloodTypeRepository = bloodTypeRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<BloodDonationDto>> GetAllAsync()
        {
            var donations = await _donationRepository.GetAllWithDetailsAsync();
            return donations.Select(MapToDto);
        }

        public async Task<BloodDonationDto> GetByIdAsync(Guid id)
        {
            var donation = await _donationRepository.GetByIdWithDetailsAsync(id);
            if (donation == null) return null;
            return MapToDto(donation);
        }

        public async Task<IEnumerable<BloodDonationDto>> GetByDonorIdAsync(Guid donorId)
        {
            var donations = await _donationRepository.GetByDonorIdAsync(donorId);
            return donations.Select(MapToDto);
        }

        public async Task<BloodDonationDto> CreateAsync(CreateBloodDonationDto dto, Guid userId)
        {
            // Lấy User từ DB
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User không tồn tại");

            // Tìm Donor theo UserId
            var donor = (await _donorRepository.GetAllAsync()).FirstOrDefault(d => d.UserId == userId);
            if (donor == null)
            {
                // Tìm BloodTypeId nếu có
                Guid? bloodTypeId = null;
                if (!string.IsNullOrWhiteSpace(dto.BloodType))
                {
                    var bloodType = (await _bloodTypeRepository.GetAllAsync())
                        .FirstOrDefault(bt => (bt.AboType + bt.RhFactor) == dto.BloodType);
                    bloodTypeId = bloodType?.BloodTypeId;
                }

                donor = new Donor
                {
                    UserId = userId,
                    FullName = user.FullName,
                    PhoneNumber = user.Phone,
                    BloodTypeId = bloodTypeId,
                    Address = dto.Address ?? string.Empty,
                    Email = user.Email,
                    CurrentMedications = dto.CurrentMedications
                };
                await _donorRepository.AddAsync(donor);
                await _donorRepository.SaveChangesAsync();
            }

            // Tạo mới BloodDonation
            var donation = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = donor.DonorId,
                RequestId = dto.RequestId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                Status = "Approved",
                Notes = dto.Notes
            };

            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();

            // Cập nhật HealthCheck "Approved" gần nhất thành "Used"
            var approvedHealthCheck = await _context.HealthChecks
                .Where(h => h.DonorId == donor.DonorId && h.HealthCheckStatus == "Approved")
                .OrderByDescending(h => h.HealthCheckDate)
                .FirstOrDefaultAsync();

            if (approvedHealthCheck != null)
            {
                approvedHealthCheck.HealthCheckStatus = "Used";
                _context.HealthChecks.Update(approvedHealthCheck);
                await _context.SaveChangesAsync();
            }

            // Build DTO trả về
            return new BloodDonationDto
            {
                DonationId = donation.DonationId,
                DonorId = donor.DonorId,
                DonationDate = dto.DonationDate,
                Quantity = donation.Quantity,
                Status = donation.Status ?? string.Empty,
                Notes = donation.Notes ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.Phone ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = dto.Address ?? string.Empty,
                BloodType = dto.BloodType ?? string.Empty,
                LastDonationDate = donor.LastDonationDate,
                CurrentMedications = dto.CurrentMedications,
                DonorName = user.FullName ?? string.Empty,
                DateOfBirth = donor.User?.DateOfBirth
            };
        }

        public async Task<BloodDonationDto> CreateWithSynchronizedInfoAsync(CreateBloodDonationDto dto)
        {
            if (!dto.DonorId.HasValue)
            {
                _logger.LogError("DonorId is required");
                throw new ArgumentException("DonorId is required");
            }
            var donor = await _donorRepository.GetByIdAsync(dto.DonorId.Value);
            if (donor == null)
            {
                _logger.LogError("Donor not found with ID: {DonorId}", dto.DonorId);
                throw new ArgumentException("Donor không tồn tại");
            }

            var user = donor.User;
            if (user == null)
            {
                _logger.LogError("User not found for donor with ID: {DonorId}", dto.DonorId);
                throw new ArgumentException("User không tồn tại cho donor này");
            }

            // Lấy thông tin nhóm máu từ Donor
            string bloodType = donor.BloodType != null
                ? $"{donor.BloodType.AboType}{donor.BloodType.RhFactor}"
                : string.Empty;

            // Tạo mới BloodDonation, ưu tiên lấy từ DTO, nếu không có thì lấy từ Donor/User
            var donation = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = dto.DonorId.Value,
                RequestId = dto.RequestId,
                DonationDate = dto.DonationDate ?? (donor.LastDonationDate.HasValue ? donor.LastDonationDate.Value.ToDateTime(TimeOnly.MinValue) : null),
                Quantity = dto.Quantity ?? 0, // Nếu Donor có trường mặc định thì lấy, còn không để 0
                Status = !string.IsNullOrWhiteSpace(dto.Status) ? dto.Status : "Pending",
                Notes = !string.IsNullOrWhiteSpace(dto.Notes) ? dto.Notes : "",
                // Các trường khác nếu cần
            };

            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();

            // Cập nhật LastDonationDate cho Donor
            if (donation.DonationDate.HasValue)
            {
                var donationDateOnly = DateOnly.FromDateTime(donation.DonationDate.Value);
                donor.LastDonationDate = donationDateOnly;
                donor.NextEligibleDate = donationDateOnly.AddMonths(3);
            }

            _donorRepository.Update(donor);
            await _donorRepository.SaveChangesAsync();

            return new BloodDonationDto
            {
                DonationId = donation.DonationId,
                DonorId = donation.DonorId ?? Guid.Empty,
                RequestId = donation.RequestId,
                DonationDate = donation.DonationDate,
                Quantity = donation.Quantity,
                Status = donation.Status ?? string.Empty,
                Notes = donation.Notes ?? string.Empty,
                CertificateId = donation.CertificateId,
                // Thông tin cá nhân từ User/Donor
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.Phone ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = donor.Address ?? string.Empty,
                BloodType = bloodType,
                LastDonationDate = donor.LastDonationDate,
                CurrentMedications = dto.CurrentMedications ?? donor.CurrentMedications ?? string.Empty,
                DonorName = user.FullName ?? string.Empty,
                RequestDescription = donation.Request?.Description ?? string.Empty,
                DateOfBirth = user.DateOfBirth
            };
        }


        public async Task<bool> UpdateAsync(Guid id, UpdateBloodDonationDto dto)
        {
            var donation = await _donationRepository.GetByIdAsync(id);
            if (donation == null) return false;

            if (dto.DonationDate.HasValue)
                donation.DonationDate = dto.DonationDate;

            if (dto.Quantity.HasValue)
                donation.Quantity = dto.Quantity;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                donation.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.Notes))
                donation.Notes = dto.Notes;

            if (dto.CertificateId.HasValue)
                donation.CertificateId = dto.CertificateId;

            //if (!string.IsNullOrWhiteSpace(dto.CurrentMedications))
            //    donation.CurrentMedications = dto.CurrentMedications;

            _donationRepository.Update(donation);
            return await _donationRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var donation = await _donationRepository.GetByIdAsync(id);
            if (donation == null) return false;

            await _donationRepository.DeleteAsync(donation);
            return await _donationRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            var donation = await _donationRepository.GetByIdAsync(id);
            if (donation == null) return false;

            donation.Status = status;
            _donationRepository.Update(donation);
            return await _donationRepository.SaveChangesAsync();
        }

        public async Task<bool> ApproveDonationAsync(Guid donationId, DateOnly? approveDate)
        {
            var donation = await _donationRepository.GetByIdAsync(donationId);
            if (donation == null) return false;
            donation.Status = "Approved";
            donation.Notes = $"Approved on {approveDate?.ToString() ?? DateTime.Now.ToShortDateString()}";
            await _donationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectDonationAsync(Guid donationId, string reason, DateOnly? rejectionDate)
        {
            var donation = await _donationRepository.GetByIdAsync(donationId);
            if (donation == null) return false;

            // Find Donor
            var donor = await _donorRepository.GetByIdAsync(donation.DonorId ?? Guid.Empty);
            if (donor == null) return false;

            // Find HealthCheck on the same date as the donation
            DateTime? donationDate = donation.DonationDate;
            HealthCheck healthCheck = null;

            if (donationDate != null)
            {
                var date = donationDate.Value.Date;
                healthCheck = await _context.HealthChecks
                    .Where(h => h.DonorId == donor.DonorId && h.HealthCheckDate.Date == date)
                    .OrderByDescending(h => h.HealthCheckDate)
                    .FirstOrDefaultAsync();
            }

            // Update old "Approved" health checks to "Used" if a donation exists for their date
            Guid excludeHealthCheckId = healthCheck != null ? healthCheck.HealthCheckId : Guid.Empty;

            var oldApprovedChecks = await _context.HealthChecks
                .Where(h => h.DonorId == donor.DonorId
                    && h.HealthCheckStatus == "Approved"
                    && h.HealthCheckId != excludeHealthCheckId)
                .ToListAsync();

            foreach (var oldCheck in oldApprovedChecks)
            {
                var used = await _context.BloodDonations.AnyAsync(d =>
                d.DonorId == donor.DonorId &&
                d.DonationDate != null &&
                d.DonationDate.Value.Date == oldCheck.HealthCheckDate.Date);
                if (used)
                {
                    oldCheck.HealthCheckStatus = "Used";
                    _context.HealthChecks.Update(oldCheck);
                }
            }

            // Update current health check status to "Rejected" if found
            if (healthCheck != null)
            {
                healthCheck.HealthCheckStatus = "Rejected";
                _context.HealthChecks.Update(healthCheck);
            }

            // Update donation status and notes
            donation.Status = "Rejected";
            donation.Notes = $"Rejected: {reason} on {rejectionDate?.ToString() ?? DateTime.Now.ToShortDateString()}";
            _donationRepository.Update(donation);

            // Save all changes
            await _context.SaveChangesAsync();
            await _donationRepository.SaveChangesAsync();

            return true;
        }


        public async Task<bool> CancelDonationAsync(Guid donationId, string reason, DateOnly? cancelDate)
        {
            var donation = await _donationRepository.GetByIdAsync(donationId);
            if (donation == null) return false;
            donation.Status = "Canceled";
            donation.Notes = $"Canceled: {reason} on {cancelDate?.ToString() ?? DateTime.Now.ToShortDateString()}";
            await _donationRepository.SaveChangesAsync();
            return true;
        }



        private BloodDonationDto MapToDto(BloodDonation donation)
        {
            return new BloodDonationDto
            {
                DonationId = donation.DonationId,
                DonorId = donation.DonorId ?? Guid.Empty,
                RequestId = donation.RequestId,
                DonationDate = donation.DonationDate,
                Quantity = donation.Quantity,
                Status = donation.Status ?? string.Empty,
                Notes = donation.Notes ?? string.Empty,
                CertificateId = donation.CertificateId,

                // Lấy thông tin từ donor (sẽ được include trong truy vấn)
                FullName = donation.Donor?.User?.FullName ?? string.Empty,
                PhoneNumber = donation.Donor?.User?.Phone ?? string.Empty,
                Email = donation.Donor?.User?.Email ?? string.Empty,
                Address = donation.Donor?.Address ?? string.Empty,

                BloodType = donation.Donor?.BloodType != null
                    ? $"{donation.Donor.BloodType.AboType}{donation.Donor.BloodType.RhFactor}"
                    : string.Empty,
                LastDonationDate = donation.Donor?.LastDonationDate,
                CurrentMedications = string.Empty, // Không có trong BloodDonation
                DonorName = donation.Donor?.User?.FullName ?? string.Empty,
                RequestDescription = donation.Request?.Description ?? string.Empty,
                UserIdCard = donation.Donor?.User?.UserIdCard ?? string.Empty,

            };
        }
    }
}
