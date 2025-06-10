using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Repositories.Interfaces;
using Services;

public class BloodRequestService : IBloodRequestService
{
    private readonly IBloodRequestRepository _requestRepository;
    private readonly IBloodRecipientRepository _recipientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BloodRequestService> _logger;

    public BloodRequestService(
        IBloodRequestRepository requestRepository,
        IBloodRecipientRepository recipientRepository,
        IUserRepository userRepository,
        ILogger<BloodRequestService> logger)
    {
        _requestRepository = requestRepository;
        _recipientRepository = recipientRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<(bool success, string message, Guid? requestId)> RegisterBloodRequestAsync(
        BloodRequestRegistrationDTO request, Guid staffId) // Thay đổi kiểu dữ liệu của staffId từ string sang Guid
    {
        try
        {
            // Kiểm tra customer có tồn tại và đúng role không
            var customer = await _userRepository.GetByIdAsync(request.PatientUserId);
            if (customer == null)
            {
                return (false, "Customer not found", null);
            }
            if (customer.Role != "Customer")
            {
                return (false, "Selected user is not a customer", null);
            }

            // Kiểm tra staff có tồn tại và đúng role không
            var staff = await _userRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Role != "Staff")
            {
                return (false, "Invalid staff member", null);
            }

            // Tìm hoặc tạo BloodRecipient mới cho customer
            var recipient = await _recipientRepository.GetByUserIdAsync(request.PatientUserId);
            if (recipient == null)
            {
                recipient = new BloodRecipient
                {
                    RecipientId = Guid.NewGuid(),
                    UserId = request.PatientUserId
                };
                await _recipientRepository.AddAsync(recipient);
                await _recipientRepository.SaveChangesAsync();
            }

            // Tạo yêu cầu máu mới
            var bloodRequest = new BloodRequest
            {
                RequestId = Guid.NewGuid(),
                RecipientId = recipient.RecipientId,
                BloodTypeRequired = request.BloodTypeRequired,
                QuantityNeeded = request.QuantityNeeded,
                UrgencyLevel = "Normal", // Mặc định
                RequestDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Pending", // Mặc định
                Description = request.Description
            };

            await _requestRepository.AddAsync(bloodRequest);
            await _requestRepository.SaveChangesAsync();

            return (true, "Blood request created successfully", bloodRequest.RequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blood request");
            return (false, "An error occurred while creating the blood request", null);
        }
    }

    public async Task<BloodRequest> GetRequestByIdAsync(Guid requestId)
    {
        return await _requestRepository.GetByIdWithDetailsAsync(requestId);
    }

    public async Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync()
    {
        return await _requestRepository.GetByStatusAsync("Pending");
    }
}