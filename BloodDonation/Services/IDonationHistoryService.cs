using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace Services
{
    public interface IDonationHistoryService
    {
        Task<IEnumerable<DonationHistoryResponseDto>> GetAllAsync();
        Task<DonationHistoryResponseDto> GetByIdAsync(Guid id);
        Task<DonationHistoryResponseDto> CreateAsync(DonationHistoryCreateDto dto);
        Task<DonationHistoryResponseDto> UpdateAsync(Guid id, DonationHistoryUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
