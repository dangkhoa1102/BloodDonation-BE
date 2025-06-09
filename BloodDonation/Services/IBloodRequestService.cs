using Models;
using Models.DTOs;

namespace Services
{
    public interface IBloodRequestService
    {
        Task<(bool success, string message, Guid? requestId)> RegisterBloodRequestAsync(
            BloodRequestRegistrationDTO request, Guid staffId); // Đổi kiểu dữ liệu của staffId từ string sang Guid
        Task<BloodRequest> GetRequestByIdAsync(Guid requestId);
        Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync();
    }
}