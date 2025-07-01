using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace Services
{
    public class DonationHistoryService : IDonationHistoryService
    {
        private readonly IDonationHistoryRepository _repo;

        public DonationHistoryService(IDonationHistoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DonationHistoryResponseDto>> GetAllAsync()
        {
            var histories = await _repo.GetAllWithDonorAsync();
            return histories.Select(MapToResponseDto);
        }

        public async Task<DonationHistoryResponseDto> GetByIdAsync(Guid id)
        {
            var history = await _repo.GetByIdWithDonorAsync(id);
            return history == null ? null : MapToResponseDto(history);
        }

        public async Task<DonationHistoryResponseDto> CreateAsync(DonationHistoryCreateDto dto)
        {
            var entity = new DonationHistory
            {
                HistoryId = Guid.NewGuid(),
                DonorId = dto.DonorId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                HealthStatus = dto.HealthStatus,
                NextEligibleDate = dto.NextEligibleDate,
                CertificateId = dto.CertificateId
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToResponseDto(entity);
        }

        public async Task<DonationHistoryResponseDto> UpdateAsync(Guid id, DonationHistoryUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.DonationDate = dto.DonationDate;
            entity.Quantity = dto.Quantity;
            entity.HealthStatus = dto.HealthStatus;
            entity.NextEligibleDate = dto.NextEligibleDate;
            entity.CertificateId = dto.CertificateId;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return MapToResponseDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _repo.Remove(entity);
            return await _repo.SaveChangesAsync();
        }

        private DonationHistoryResponseDto MapToResponseDto(DonationHistory history)
        {
            return new DonationHistoryResponseDto
            {
                HistoryId = history.HistoryId,
                DonorId = history.DonorId ?? Guid.Empty,
                DonationDate = history.DonationDate ?? default,
                Quantity = history.Quantity ?? 0,
                HealthStatus = history.HealthStatus ?? string.Empty,
                NextEligibleDate = history.NextEligibleDate ?? default,
                CertificateId = history.CertificateId,
                DonorName = history.Donor?.FullName,
                DonorBloodType = history.Donor?.BloodType?.ToString()
            };
        }
    }
}
