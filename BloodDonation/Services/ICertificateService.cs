using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;

namespace Services
{
    public interface ICertificateService
    {
        Task<IEnumerable<CertificateDto>> GetAllAsync();
        Task<CertificateDto?> GetByIdAsync(Guid id);
        Task<CertificateDto> CreateAsync(CreateCertificateDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateCertificateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
