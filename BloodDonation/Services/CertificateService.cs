using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Services
{

    // Service Implementation
    public class CertificateService : ICertificateService
    {
        private readonly BloodDonationSupportContext _context;

        public CertificateService(BloodDonationSupportContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CertificateDto>> GetAllAsync()
        {
            return await _context.Certificates
                .Include(c => c.Donor)
                    .ThenInclude(d => d.User)
                .Include(c => c.Donor)
                    .ThenInclude(d => d.BloodType)
                .Include(c => c.Donation)
                .Select(c => new CertificateDto
                {
                    CertificateId = c.CertificateId,
                    DonorId = c.DonorId,
                    DonationId = c.DonationId,
                    StaffId = c.StaffId,
                    CertificateNumber = c.CertificateNumber,
                    IssueDate = c.IssueDate,
                    CertificateType = c.CertificateType,
                    CreatedDate = c.CreatedDate,
                    LastModified = c.LastModified,

                    // Thông tin mở rộng
                    FullName = c.Donor != null ? c.Donor.FullName : null,
                    UserIdCard = c.Donor != null && c.Donor.User != null ? c.Donor.User.UserIdCard : null,
                    DateOfBirth = c.Donor != null && c.Donor.User != null ? c.Donor.User.DateOfBirth : null,
                    Address = c.Donor != null && c.Donor.User != null ? c.Donor.Address : null,
                    Quantity = c.Donation != null ? c.Donation.Quantity : null,
                    BloodType = c.Donor != null && c.Donor.BloodType != null
                            ? c.Donor.BloodType.AboType + c.Donor.BloodType.RhFactor
                            : null,
                    BloodDonationDate = c.Donation != null ? c.Donation.DonationDate : null
                })
                .ToListAsync();
        }

        public async Task<CertificateDto?> GetByIdAsync(Guid id)
        {
            var c = await _context.Certificates
                .Include(c => c.Donor)
                    .ThenInclude(d => d.User)
                .Include(c => c.Donor)
                    .ThenInclude(d => d.BloodType)
                .Include(c => c.Donation)
                .FirstOrDefaultAsync(c => c.CertificateId == id);

            if (c == null) return null;

            return new CertificateDto
            {
                CertificateId = c.CertificateId,
                DonorId = c.DonorId,
                DonationId = c.DonationId,
                StaffId = c.StaffId,
                CertificateNumber = c.CertificateNumber,
                IssueDate = c.IssueDate,
                CertificateType = c.CertificateType,
                CreatedDate = c.CreatedDate,
                LastModified = c.LastModified,

                FullName = c.Donor != null ? c.Donor.FullName : null,
                UserIdCard = c.Donor != null && c.Donor.User != null ? c.Donor.User.UserIdCard : null,
                DateOfBirth = c.Donor != null && c.Donor.User != null ? c.Donor.User.DateOfBirth : null,
                Address = c.Donor != null && c.Donor.User != null ? c.Donor.Address : null,
                Quantity = c.Donation != null ? c.Donation.Quantity : null,
                BloodType = c.Donor != null && c.Donor.BloodType != null
                            ? c.Donor.BloodType.AboType + c.Donor.BloodType.RhFactor
                            : null,
                BloodDonationDate = c.Donation != null ? c.Donation.DonationDate : null
            };
        }

        public async Task<CertificateDto> CreateAsync(CreateCertificateDto dto)
        {
            var now = DateTime.Now;
            var entity = new Certificate
            {
                CertificateId = Guid.NewGuid(),
                DonorId = dto.DonorId,
                DonationId = dto.DonationId,
                StaffId = dto.StaffId,
                CertificateNumber = dto.CertificateNumber,
                IssueDate = dto.IssueDate,
                CertificateType = dto.CertificateType,
                CreatedDate = now,
                LastModified = now
            };
            _context.Certificates.Add(entity);
            await _context.SaveChangesAsync();

            return new CertificateDto
            {
                CertificateId = entity.CertificateId,
                DonorId = entity.DonorId,
                DonationId = entity.DonationId,
                StaffId = entity.StaffId,
                CertificateNumber = entity.CertificateNumber,
                IssueDate = entity.IssueDate,
                CertificateType = entity.CertificateType,
                CreatedDate = entity.CreatedDate,
                LastModified = entity.LastModified
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateCertificateDto dto)
        {
            var entity = await _context.Certificates.FindAsync(id);
            if (entity == null) return false;

            entity.CertificateNumber = dto.CertificateNumber;
            entity.IssueDate = dto.IssueDate;
            entity.CertificateType = dto.CertificateType;
            entity.LastModified = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Certificates.FindAsync(id);
            if (entity == null) return false;
            _context.Certificates.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
