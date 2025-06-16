using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Enums;
using Repositories.Implementations;
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
        BloodRequestRegistrationDTO request, Guid staffId)
    {
        try
        {
            var member = await _userRepository.GetByIdAsync(request.PatientUserId);
            if (member == null)
            {
                return (false, "Member not found", null);  
            }
            if (member.Role != "Member")  
            {
                return (false, "Selected user is not a member", null);
            }
            var staff = await _userRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Role != "Staff")
            {
                return (false, "Invalid staff member", null);
            }
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
            var bloodRequest = new BloodRequest
            {
                RequestId = Guid.NewGuid(),
                RecipientId = recipient.RecipientId,
                BloodTypeRequired = request.BloodTypeRequired,
                QuantityNeeded = request.QuantityNeeded,
                UrgencyLevel = "Normal",
                RequestDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Pending",
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
                .ToList();

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
           
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != "Member") 
            {
                return Enumerable.Empty<BloodRequest>();
            }
            var recipient = await _recipientRepository.GetByUserIdAsync(userId);
            if (recipient == null)
            {
                return Enumerable.Empty<BloodRequest>();
            }
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
    public async Task<(bool success, string message)> UpdateBloodRequestAsync(
    Guid requestId, BloodRequestUpdateDTO updateDto, Guid staffId)
    {
        try
        {
            var staff = await _userRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Role != "Staff")
            {
                return (false, "Invalid staff member");
            }

            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return (false, "Blood request not found");
            }
            if (updateDto.BloodTypeRequired.HasValue)
                request.BloodTypeRequired = updateDto.BloodTypeRequired;

            if (updateDto.QuantityNeeded.HasValue)
                request.QuantityNeeded = updateDto.QuantityNeeded;

            if (!string.IsNullOrWhiteSpace(updateDto.UrgencyLevel))
                request.UrgencyLevel = updateDto.UrgencyLevel;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                request.Status = updateDto.Status;

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
                request.Description = updateDto.Description;

            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return (true, "Blood request updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blood request");
            return (false, "An error occurred while updating the blood request");
        }
    }
    public async Task<(bool success, string message)> RejectBloodRequestAsync(
    Guid requestId,
    BloodRequestRejectDTO rejectDto,
    Guid staffId)
    {
        try
        {
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);

            if (request == null)
            {
                return (false, "Blood request not found");
            }

            // Chỉ cho phép từ chối đơn có trạng thái Pending
            if (request.Status != BloodRequestStatus.Pending.ToString())
            {
                return (false, $"Can only reject requests with 'Pending' status. Current status: {request.Status}");
            }

            // Cập nhật trạng thái và thông tin từ chối
            request.Status = BloodRequestStatus.Rejected.ToString();
            request.Description = $"Reason: {rejectDto.RejectionReason}";

            // Tạo notification cho recipient
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Request Rejection",
                Message = $"Your blood request (ID: {request.RequestId}) has been rejected.\nReason: {rejectDto.RejectionReason}",
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            // Log hoạt động
            _logger.LogInformation(
                "Blood request {RequestId} rejected by staff {StaffId}. Reason: {Reason}",
                requestId, staffId, rejectDto.RejectionReason);

            // Lưu thay đổi
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return (true, "Blood request rejected successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting blood request {RequestId}", requestId);
            return (false, "An error occurred while rejecting the blood request");
        }
    }
}