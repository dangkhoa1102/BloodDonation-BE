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
    private readonly IBloodTypeRepository _bloodTypeRepository;
    private readonly ILogger<BloodRequestService> _logger;

    public BloodRequestService(
        IBloodRequestRepository requestRepository,
        IBloodRecipientRepository recipientRepository,
        IUserRepository userRepository,
        IBloodTypeRepository bloodTypeRepository,
        ILogger<BloodRequestService> logger)
    {
        _requestRepository = requestRepository;
        _recipientRepository = recipientRepository;
        _userRepository = userRepository;
        _bloodTypeRepository = bloodTypeRepository;
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
            _logger.LogInformation($"Getting requests with status: {statusString}");

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
                Message = $"Your blood request (ID: {request.RequestId}) has been rejected.\n{rejectDto.RejectionReason}",
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
    public async Task<(bool success, string message)> ApproveBloodRequestAsync(
    Guid requestId,
    Guid staffId)
    {
        try
        {
            // Kiểm tra staff
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

            // Chỉ cho phép approve đơn có trạng thái Pending
            if (request.Status != BloodRequestStatus.Pending.ToString())
            {
                return (false, $"Can only approve requests with 'Pending' status. Current status: {request.Status}");
            }

            // Cập nhật trạng thái 
            request.Status = BloodRequestStatus.Approved.ToString();

            // Tạo notification cho recipient
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Request Approved",
                Message = $"Your blood request (ID: {request.RequestId}) has been approved.",
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            // Log hoạt động
            _logger.LogInformation(
                "Blood request {RequestId} approved by staff {StaffId}",
                requestId, staffId);

            // Lưu thay đổi
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return (true, "Blood request approved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving blood request {RequestId}", requestId);
            return (false, "An error occurred while approving the blood request");
        }

    }
    public async Task<(bool success, string message, Guid? requestId)> RegisterEmergencyBloodRequestAsync(
    EmergencyBloodRequestDTO request, Guid staffId)
    {
        try
        {
            _logger.LogInformation("Starting emergency blood request registration. Staff ID: {StaffId}", staffId);

            var staff = await _userRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Role != "Staff")
            {
                return (false, "Invalid staff member", null);
            }

            // Check blood inventory first
            var bloodTypeId = request.BloodTypeRequired;
            var quantityNeeded = request.QuantityNeeded;
            var availableQuantity = await _bloodTypeRepository.GetAvailableUnitsQuantityAsync(bloodTypeId);
            // Xác định trạng thái dựa trên số lượng máu khả dụng (ml)
            var initialStatus = availableQuantity >= quantityNeeded
                ? BloodRequestStatus.Pending.ToString()  // Có đủ ml máu trong kho
                : BloodRequestStatus.Opened.ToString();   // Không đủ ml máu, cần hiển thị lên UI
            _logger.LogInformation(
            "Blood availability check - Required: {Required}ml, Available: {Available}ml, Status: {Status}",
            quantityNeeded, availableQuantity, initialStatus);
            // Create or update user
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = request.Email,
                    Username = request.Email.Split('@')[0],
                    Password = "benhnhan123",
                    FullName = request.PatientName,
                    UserIdCard = request.UserIdCard,
                    Phone = request.Phone,
                    DateOfBirth = request.DateOfBirth,
                    Role = UserRoles.Member.ToString()
                };

                await _userRepository.AddAsync(existingUser);
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation("Created new user with ID: {UserId}", existingUser.UserId);
            }
            else
            {
                existingUser.FullName = request.PatientName;
                existingUser.UserIdCard = request.UserIdCard;
                existingUser.Phone = request.Phone;
                if (request.DateOfBirth.HasValue)
                {
                    existingUser.DateOfBirth = request.DateOfBirth;
                }
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation("Updated existing user information for ID: {UserId}", existingUser.UserId);
            }

            // Create or get recipient
            var recipient = await _recipientRepository.GetByUserIdAsync(existingUser.UserId);
            if (recipient == null)
            {
                recipient = new BloodRecipient
                {
                    RecipientId = Guid.NewGuid(),
                    UserId = existingUser.UserId,
                    ContactInfo = request.Phone
                };
                await _recipientRepository.AddAsync(recipient);
                await _recipientRepository.SaveChangesAsync();
                _logger.LogInformation("Created new recipient with ID: {RecipientId}", recipient.RecipientId);
            }

            // Create emergency blood request
            var bloodRequest = new BloodRequest
            {
                RequestId = Guid.NewGuid(),
                RecipientId = recipient.RecipientId,
                BloodTypeRequired = request.BloodTypeRequired,
                QuantityNeeded = request.QuantityNeeded,
                UrgencyLevel = "Emergency",
                RequestDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = initialStatus,
                Description = $"Emergency request. Available quantity: {availableQuantity}ml. {request.Description}"
            };

            await _requestRepository.AddAsync(bloodRequest);
            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Created emergency blood request with ID: {RequestId}, Status: {Status}, Available Units: {AvailableUnits}",
                bloodRequest.RequestId, initialStatus, availableQuantity);

            // Create notification
            var notificationMessage = initialStatus == BloodRequestStatus.Pending.ToString()
                ? $"Emergency blood request has been created with sufficient blood in inventory. Request ID: {bloodRequest.RequestId}"
                : $"Emergency blood request has been created and opened for donors. Request ID: {bloodRequest.RequestId}";

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = existingUser.UserId,
                NotificationType = "Emergency Blood Request",
                Message = notificationMessage,
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            return (true, $"Emergency blood request created successfully with status: {initialStatus}", bloodRequest.RequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating emergency blood request");
            return (false, $"An error occurred: {ex.Message}", null);
        }
    }
    private bool IsValidStatusTransition(BloodRequestStatus currentStatus, BloodRequestStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (BloodRequestStatus.Draft, BloodRequestStatus.Pending) => true,
            (BloodRequestStatus.Pending, BloodRequestStatus.Opened) => true,
            (BloodRequestStatus.Pending, BloodRequestStatus.Approved) => true,
            (BloodRequestStatus.Pending, BloodRequestStatus.Rejected) => true,
            (BloodRequestStatus.Opened, BloodRequestStatus.Approved) => true,
            (BloodRequestStatus.Opened, BloodRequestStatus.Rejected) => true,
            (BloodRequestStatus.Approved, BloodRequestStatus.Done) => true,
            (BloodRequestStatus.Approved, BloodRequestStatus.Closed) => true,
            (BloodRequestStatus.Done, BloodRequestStatus.Closed) => true,
            _ => false
        };
    }
    public async Task<(bool success, string message)> UpdateRequestStatusAsync(
    Guid requestId,
    BloodRequestStatus newStatus,
    Guid userId)
    {
        try
        {
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return (false, "Blood request not found");
            }

            // Parse current status
            if (!Enum.TryParse<BloodRequestStatus>(request.Status, out var currentStatus))
            {
                return (false, "Invalid current status");
            }

            // Validate status transition
            if (!IsValidStatusTransition(currentStatus, newStatus))
            {
                return (false, $"Invalid status transition from {currentStatus} to {newStatus}");
            }

            // Update status
            request.Status = newStatus.ToString();

            // Create notification
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Blood Request Status Update",
                Message = $"Your blood request (ID: {requestId}) status has been updated to {newStatus}",
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            // Save changes
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return (true, $"Blood request status updated to {newStatus} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blood request {RequestId} status to {NewStatus}",
                requestId, newStatus);
            return (false, "An error occurred while updating the request status");
        }
    }
    public async Task<(bool success, string message)> UpdateReceivedQuantityAsync(
    BloodRequestUpdateQuantityDTO updateDto,
    Guid staffId)
    {
        try
        {
            // Kiểm tra staff
            var staff = await _userRepository.GetByIdAsync(staffId);
            if (staff == null || staff.Role != "Staff")
            {
                return (false, "Invalid staff member");
            }

            var request = await _requestRepository.GetByIdWithDetailsAsync(updateDto.RequestId);
            if (request == null)
            {
                return (false, "Blood request not found");
            }

            // Tính toán số lượng còn lại cần nhận
            var remainingQuantity = request.QuantityNeeded - updateDto.ReceivedQuantity;

            if (remainingQuantity < 0)
            {
                return (false, "Received quantity cannot exceed needed quantity");
            }

            // Cập nhật số lượng còn lại
            request.QuantityNeeded = remainingQuantity;

            // Nếu đã nhận đủ số lượng (remainingQuantity = 0)
            if (remainingQuantity == 0)
            {
                request.Status = BloodRequestStatus.Done.ToString();
            }

            // Tạo notification cho recipient
            var notificationMessage = remainingQuantity == 0
                ? $"Your blood request (ID: {request.RequestId}) has been completed. All required blood has been received."
                : $"Your blood request (ID: {request.RequestId}) has received {updateDto.ReceivedQuantity}ml of blood. Remaining needed: {remainingQuantity}ml";

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Blood Request Update",
                Message = notificationMessage,
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            // Log hoạt động
            _logger.LogInformation(
                "Blood request {RequestId} updated by staff {StaffId}. Received: {ReceivedQuantity}ml, Remaining: {RemainingQuantity}ml",
                request.RequestId, staffId, updateDto.ReceivedQuantity, remainingQuantity);

            // Lưu thay đổi
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return (true, remainingQuantity == 0
                ? "Blood request completed successfully"
                : $"Blood request updated successfully. Remaining quantity needed: {remainingQuantity}ml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blood request received quantity {RequestId}", updateDto.RequestId);
            return (false, "An error occurred while updating the blood request quantity");
        }
    }
}