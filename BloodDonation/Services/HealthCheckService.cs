using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Repositories.Interfaces;
using System.Drawing;

namespace Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _repo;
        private readonly IDonorRepository _donorRepo;
        public HealthCheckService(IHealthCheckRepository repo, IDonorRepository donorRepo)
        {
            _repo = repo;
            _donorRepo = donorRepo;
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
    }
}
