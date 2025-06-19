using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Repositories.Interfaces;
using static Models.DTOs.DonorDTO;
using Services;
using Microsoft.AspNetCore.Mvc;


namespace Services
{
    public class DonorService : IDonorService
    {
        private readonly IGenericRepository<Donor> _repository;

        public DonorService(IGenericRepository<Donor> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DonorDto>> GetAllAsync()
        {
            var donors = await _repository.GetAllAsync();
            return donors.Select(d => new DonorDto
            {
                DonorId = d.DonorId,
                UserId = d.UserId,
                BloodTypeId = d.BloodTypeId,
                Weight = d.Weight,
                Height = d.Height,
                MedicalHistory = d.MedicalHistory,
                IsAvailable = d.IsAvailable,
                LastDonationDate = d.LastDonationDate,
                NextEligibleDate = d.NextEligibleDate,
                LocationId = d.LocationId,
                ClosestFacilityId = d.ClosestFacilityId
            });
        }

        public async Task<DonorDto> GetByIdAsync(Guid id)
        {
            var d = await _repository.GetByIdAsync(id);
            if (d == null) return null;

            return new DonorDto
            {
                DonorId = d.DonorId,
                UserId = d.UserId,
                BloodTypeId = d.BloodTypeId,
                Weight = d.Weight,
                Height = d.Height,
                MedicalHistory = d.MedicalHistory,
                IsAvailable = d.IsAvailable,
                LastDonationDate = d.LastDonationDate,
                NextEligibleDate = d.NextEligibleDate,
                LocationId = d.LocationId,
                ClosestFacilityId = d.ClosestFacilityId
            };
        }

        public async Task<DonorDto> CreateAsync(CreateDonorDto dto)
        {
            var entity = new Donor
            {
                DonorId = Guid.NewGuid(),
                UserId = dto.UserId,
                BloodTypeId = dto.BloodTypeId,
                Weight = dto.Weight,
                Height = dto.Height,
                MedicalHistory = dto.MedicalHistory,
                IsAvailable = dto.IsAvailable,
                LastDonationDate = dto.LastDonationDate,
                NextEligibleDate = dto.NextEligibleDate,
                LocationId = dto.LocationId,
                ClosestFacilityId = dto.ClosestFacilityId
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new DonorDto
            {
                DonorId = entity.DonorId,
                UserId = entity.UserId,
                BloodTypeId = entity.BloodTypeId,
                Weight = entity.Weight,
                Height = entity.Height,
                MedicalHistory = entity.MedicalHistory,
                IsAvailable = entity.IsAvailable,
                LastDonationDate = entity.LastDonationDate,
                NextEligibleDate = entity.NextEligibleDate,
                LocationId = entity.LocationId,
                ClosestFacilityId = entity.ClosestFacilityId
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDonorDto dto)
        {
            var donor = await _repository.GetByIdAsync(id);
            if (donor == null) return false;

            donor.UserId = dto.UserId;
            donor.BloodTypeId = dto.BloodTypeId;
            donor.Weight = dto.Weight;
            donor.Height = dto.Height;
            donor.MedicalHistory = dto.MedicalHistory;
            donor.IsAvailable = dto.IsAvailable;
            donor.LastDonationDate = dto.LastDonationDate;
            donor.NextEligibleDate = dto.NextEligibleDate;
            donor.LocationId = dto.LocationId;
            donor.ClosestFacilityId = dto.ClosestFacilityId;

            _repository.Update(donor);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var donor = await _repository.GetByIdAsync(id);
            if (donor == null) return false;

            _repository.Remove(donor);
            return await _repository.SaveChangesAsync();
        }
    }
}
