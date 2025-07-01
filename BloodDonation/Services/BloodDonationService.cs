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

        public BloodDonationService(
            IBloodDonationRepository donationRepository,
            IDonorRepository donorRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<Location> locationRepository,
            IGenericRepository<BloodType> bloodTypeRepository,
            ILogger<BloodDonationService> logger)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _bloodTypeRepository = bloodTypeRepository;
            _logger = logger;
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
                    // Email không cần vì đã có trong User
                    BloodTypeId = bloodTypeId,
                    Address = dto.Address,
                    Email = user.Email,
                    CurrentMedications = dto.CurrentMedications
                    // Nếu có Address, bạn có thể lưu vào Location hoặc trường riêng
                };
                await _donorRepository.AddAsync(donor);
                await _donorRepository.SaveChangesAsync();
            }

            // Tạo mới BloodDonation
            var donation = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = donor.DonorId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                Status = dto.Status,
                Notes = dto.Notes
            };

            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();

            // Build DTO trả về
            return new BloodDonationDto
            {
                DonationId = donation.DonationId,
                DonorId = donor.DonorId,
                DonationDate = donation.DonationDate,
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
            // Bằng
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

            // Lấy thông tin địa chỉ từ Location nếu có
            string address = "";
            if (donor.Location != null)
            {
                address = $"{donor.Location.Address}, {donor.Location.District}, {donor.Location.City}";
            }

            // Lấy thông tin nhóm máu
            string bloodType = "";
            if (donor.BloodType != null)
            {
                bloodType = $"{donor.BloodType.AboType}{donor.BloodType.RhFactor}";
            }

            var donation = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = dto.DonorId.Value,
                RequestId = dto.RequestId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                Status = dto.Status,
                Notes = dto.Notes,

                //// Thông tin cá nhân đồng bộ từ User
                //FullName = user.FullName,
                //PhoneNumber = user.Phone,
                //Email = user.Email,
                //Address = address,

                //// Thông tin y tế đồng bộ từ Donor
                //BloodType = bloodType,
                //LastDonationDate = donor.LastDonationDate,
                ////MedicalHistory = donor.MedicalHistory,
                //CurrentMedications = dto.CurrentMedications
            };

            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();

            // Cập nhật LastDonationDate cho Donor
            if (dto.DonationDate.HasValue)
            {
                donor.LastDonationDate = dto.DonationDate;

                // Tính toán NextEligibleDate (ví dụ: 3 tháng sau lần hiến máu)
                donor.NextEligibleDate = DateOnly.FromDateTime(
                    dto.DonationDate.Value.ToDateTime(TimeOnly.MinValue).AddMonths(3));
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

                // Lấy thông tin từ Donor và User
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.Phone ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = donor.Location != null
            ? $"{donor.Location.Address}, {donor.Location.District}, {donor.Location.City}"
            : string.Empty,
                BloodType = donor.BloodType != null
            ? $"{donor.BloodType.AboType}{donor.BloodType.RhFactor}"
            : string.Empty,
                LastDonationDate = donor.LastDonationDate,
                CurrentMedications = dto.CurrentMedications ?? string.Empty,
                DonorName = user.FullName ?? string.Empty,
                RequestDescription = donation.Request?.Description ?? string.Empty,
                DateOfBirth = donor.User?.DateOfBirth
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
                Address = donation.Donor?.Location != null
                    ? $"{donation.Donor.Location.Address}, {donation.Donor.Location.District}, {donation.Donor.Location.City}"
                    : string.Empty,
                BloodType = donation.Donor?.BloodType != null
                    ? $"{donation.Donor.BloodType.AboType}{donation.Donor.BloodType.RhFactor}"
                    : string.Empty,
                LastDonationDate = donation.Donor?.LastDonationDate,
                CurrentMedications = string.Empty, // Không có trong BloodDonation
                DonorName = donation.Donor?.User?.FullName ?? string.Empty,
                RequestDescription = donation.Request?.Description ?? string.Empty,

            };
        }
    }
}
