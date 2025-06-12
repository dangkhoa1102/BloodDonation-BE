using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Enums;
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

    

    public async Task<IEnumerable<BloodRequest>> GetAllRequestsAsync()
    {
        try
        {
            var requests = await _requestRepository.GetAllAsync();
            // Sort by date descending to show newest first
            return requests.OrderByDescending(r => r.RequestDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blood requests");
            return Enumerable.Empty<BloodRequest>();
        }
    }
    public async Task<BloodRequest> GetRequestByIdAsync(Guid requestId)
    {
        return await _requestRepository.GetByIdWithDetailsAsync(requestId);
    }

    public async Task<IEnumerable<BloodRequest>> GetRequestsByStatusAsync(BloodRequestStatus status)
    {
        try
        {
            var statusString = status.ToString();
            var requests = await _requestRepository.GetByStatusAsync(statusString);
            return requests.OrderByDescending(r => r.RequestDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blood requests by status: {Status}", status);
            return Enumerable.Empty<BloodRequest>();
        }
    }

    public async Task<IEnumerable<BloodRequest>> GetRequestsByRecipientNameAsync(string recipientName)
    {
        try
        {
            var requests = await _requestRepository.GetAllAsync();
            var filteredRequests = requests
                .Join(_recipientRepository.GetAllAsync().Result,
                    request => request.RecipientId,
                    recipient => recipient.RecipientId,
                    (request, recipient) => new { Request = request, Recipient = recipient })
                .Join(_userRepository.GetAllAsync().Result,
                    joined => joined.Recipient.UserId,
                    user => user.UserId,
                    (joined, user) => new { joined.Request, joined.Recipient, User = user })
                .Where(joined => joined.User.FullName.ToLower().Contains(recipientName.ToLower()))
                .Select(joined => joined.Request)
                .ToList(); // Changed from ToListAsync() to ToList()

            return filteredRequests.OrderByDescending(r => r.RequestDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blood requests by recipient name: {RecipientName}", recipientName);
            return Enumerable.Empty<BloodRequest>();
        }
    }
    public async Task<IEnumerable<BloodRequest>> GetRequestsByRecipientUserIdAsync(Guid userId)
    {
        try
        {
            // Kiểm tra xem user có tồn tại và có phải là Customer không
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != "Customer")
            {
                return Enumerable.Empty<BloodRequest>();
            }

            // Lấy BloodRecipient của user
            var recipient = await _recipientRepository.GetByUserIdAsync(userId);
            if (recipient == null)
            {
                return Enumerable.Empty<BloodRequest>();
            }

            // Lấy các request của recipient và sắp xếp theo ngày mới nhất
            var requests = await _requestRepository.GetAllAsync();
            var recipientRequests = requests
                .Where(r => r.RecipientId == recipient.RecipientId)
                .OrderByDescending(r => r.RequestDate);

            return recipientRequests;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blood requests for recipient user: {UserId}", userId);
            return Enumerable.Empty<BloodRequest>();
        }
    }
}