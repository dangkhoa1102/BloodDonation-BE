using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Repositories.Interfaces;

namespace Services
{

    // Service Implementation
    public class CertificateService : ICertificateService
    {
        private readonly IGenericRepository<Certificate> _repository;

        public CertificateService(IGenericRepository<Certificate> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CertificateDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(x => new CertificateDto
            {
                CertificateId = x.CertificateId,
                DonorId = x.DonorId,
                DonationId = x.DonationId,
                StaffId = x.StaffId,
                CertificateNumber = x.CertificateNumber,
                IssueDate = x.IssueDate,
                CertificateType = x.CertificateType
            });
        }

        public async Task<CertificateDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new CertificateDto
            {
                CertificateId = entity.CertificateId,
                DonorId = entity.DonorId,
                DonationId = entity.DonationId,
                StaffId = entity.StaffId,
                CertificateNumber = entity.CertificateNumber,
                IssueDate = entity.IssueDate,
                CertificateType = entity.CertificateType
            };
        }

        public async Task<CertificateDto> CreateAsync(CreateCertificateDto dto)
        {
            var entity = new Certificate
            {
                CertificateId = Guid.NewGuid(),
                DonorId = dto.DonorId,
                DonationId = dto.DonationId,
                StaffId = dto.StaffId,
                CertificateNumber = dto.CertificateNumber,
                IssueDate = dto.IssueDate,
                CertificateType = dto.CertificateType,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new CertificateDto
            {
                CertificateId = entity.CertificateId,
                DonorId = entity.DonorId,
                DonationId = entity.DonationId,
                StaffId = entity.StaffId,
                CertificateNumber = entity.CertificateNumber,
                IssueDate = entity.IssueDate,
                CertificateType = entity.CertificateType
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateCertificateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.CertificateNumber = dto.CertificateNumber;
            existing.IssueDate = dto.IssueDate;
            existing.CertificateType = dto.CertificateType;
            existing.LastModified = DateTime.UtcNow;

            _repository.Update(existing);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            _repository.Remove(existing);
            return await _repository.SaveChangesAsync();
        }
    }
}
