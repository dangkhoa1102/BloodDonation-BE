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


namespace Services
{
    public class BloodDonationService : IBloodDonationService
    {
        private readonly IBloodDonationRepository _repository;

        public BloodDonationService(IBloodDonationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BloodDonationDto>> GetAllAsync()
        {
            var donations = await _repository.GetAllAsync();
            return donations.Select(d => new BloodDonationDto
            {
                DonationId = d.DonationId,
                DonorId = d.DonorId,
                RequestId = d.RequestId,
                DonationDate = d.DonationDate,
                Quantity = d.Quantity,
                Status = d.Status,
                Notes = d.Notes
            });
        }

        public async Task<BloodDonationDto?> GetByIdAsync(Guid id)
        {
            var d = await _repository.GetByIdAsync(id);
            if (d == null) return null;
            return new BloodDonationDto
            {
                DonationId = d.DonationId,
                DonorId = d.DonorId,
                RequestId = d.RequestId,
                DonationDate = d.DonationDate,
                Quantity = d.Quantity,
                Status = d.Status,
                Notes = d.Notes
            };
        }

        public async Task<BloodDonationDto> CreateAsync(CreateBloodDonationDto dto)
        {
            var entity = new BloodDonation
            {
                DonationId = Guid.NewGuid(),
                DonorId = dto.DonorId,
                RequestId = dto.RequestId,
                DonationDate = dto.DonationDate,
                Quantity = dto.Quantity,
                Status = dto.Status,
                Notes = dto.Notes
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new BloodDonationDto
            {
                DonationId = entity.DonationId,
                DonorId = entity.DonorId,
                RequestId = entity.RequestId,
                DonationDate = entity.DonationDate,
                Quantity = entity.Quantity,
                Status = entity.Status,
                Notes = entity.Notes
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateBloodDonationDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.DonationDate = dto.DonationDate;
            existing.Quantity = dto.Quantity;
            existing.Status = dto.Status;
            existing.Notes = dto.Notes;

            _repository.Update(existing);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var donation = await _repository.GetByIdAsync(id);
            if (donation == null) return false;

            _repository.Remove(donation);
            return await _repository.SaveChangesAsync();
        }
    }

}
