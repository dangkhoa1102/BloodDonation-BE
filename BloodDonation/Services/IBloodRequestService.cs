using Models;
using Models.DTOs;
using Models.Enums;

namespace Services
{
    public interface IBloodRequestService
    {
        Task<(bool success, string message, Guid? requestId)> RegisterBloodRequestAsync(
            BloodRequestRegistrationDTO request, Guid staffId);
        Task<BloodRequest> GetRequestByIdAsync(Guid requestId);
        Task<IEnumerable<BloodRequest>> GetAllRequestsAsync();
        Task<IEnumerable<BloodRequest>> GetRequestsByStatusAsync(BloodRequestStatus status);
        Task<IEnumerable<BloodRequest>> GetRequestsByRecipientNameAsync(string recipientName);
        Task<IEnumerable<BloodRequest>> GetRequestsByRecipientUserIdAsync(Guid userId);
        Task<(bool success, string message)> UpdateBloodRequestAsync(Guid requestId, BloodRequestUpdateDTO updateDto, Guid staffId);
        Task<(bool success, string message)> RejectBloodRequestAsync(Guid requestId,BloodRequestRejectDTO rejectDto,Guid staffId);
        Task<(bool success, string message)> ApproveBloodRequestAsync(Guid requestId, Guid staffId);
        Task<(bool success, string message, Guid? requestId)> RegisterEmergencyBloodRequestAsync(
            EmergencyBloodRequestDTO request, Guid staffId);
        Task<(bool success, string message)> UpdateRequestStatusAsync(
            Guid requestId,
            BloodRequestStatus newStatus,
            Guid staffId);
       Task<(bool success, string message)> UpdateReceivedQuantityAsync(
       BloodRequestUpdateQuantityDTO updateDto,
       Guid staffId);
        Task<UserDetailDTO> GetRequestUserDetailsAsync(Guid requestId);
    }
}