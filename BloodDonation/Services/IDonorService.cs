using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using static Models.DTOs.DonorDTO;

namespace Services
{
    public interface IDonorService
    {
        Task<IEnumerable<DonorDto>> GetAllAsync();
        Task<DonorDto> GetByIdAsync(Guid id);
        Task<DonorDto> CreateAsync(CreateDonorDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateDonorDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

}
