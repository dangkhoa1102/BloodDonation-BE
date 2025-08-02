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
    private readonly IDonorRepository _donorRepository;

    public BloodRequestService(
        IBloodRequestRepository requestRepository,
        IBloodRecipientRepository recipientRepository,
        IUserRepository userRepository,
        IBloodTypeRepository bloodTypeRepository,
        ILogger<BloodRequestService> logger,
        IDonorRepository donorRepository)
    {
        _requestRepository = requestRepository;
        _recipientRepository = recipientRepository;
        _userRepository = userRepository;
        _bloodTypeRepository = bloodTypeRepository;
        _logger = logger;
        _donorRepository = donorRepository;
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

            // Tìm hoặc tạo user mới
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

            // Kiểm tra xem user đã có donor profile với nhóm máu chưa
            var existingDonor = await _donorRepository.GetByIdAsync(existingUser.UserId);
            Guid bloodTypeId;

            if (existingDonor != null && existingDonor.BloodTypeId.HasValue)
            {
                // Nếu donor đã có nhóm máu, sử dụng nhóm máu đó
                bloodTypeId = existingDonor.BloodTypeId.Value;
                _logger.LogInformation("Using existing blood type for user {UserId}: {BloodTypeId}", 
                    existingUser.UserId, bloodTypeId);
            }
            else
            {
                // Nếu chưa có donor hoặc chưa có nhóm máu, sử dụng nhóm máu từ yêu cầu
                bloodTypeId = request.BloodTypeRequired;
                
                if (existingDonor == null)
                {
                    // Tạo mới donor profile
                    existingDonor = new Donor
                    {
                        DonorId = Guid.NewGuid(),
                        UserId = existingUser.UserId,
                        BloodTypeId = bloodTypeId,
                        FullName = request.PatientName,
                        Email = request.Email,
                        PhoneNumber = request.Phone
                    };
                    await _donorRepository.AddAsync(existingDonor);
                }
                else
                {
                    // Cập nhật nhóm máu cho donor hiện có
                    existingDonor.BloodTypeId = bloodTypeId;
                    _donorRepository.Update(existingDonor);
                }
                await _donorRepository.SaveChangesAsync();
                _logger.LogInformation("Updated blood type for user {UserId} to {BloodTypeId}", 
                    existingUser.UserId, bloodTypeId);
            }

            // Lấy thông tin nhóm máu
            var requiredBloodType = await _bloodTypeRepository.GetByIdAsync(bloodTypeId);
            if (requiredBloodType == null)
            {
                return (false, "Invalid blood type specified", null);
            }

            var bloodTypeString = $"{requiredBloodType.AboType}{requiredBloodType.RhFactor}";
            var quantityNeeded = request.QuantityNeeded;
            var totalAvailableQuantity = 0;

            // Kiểm tra và tính toán lượng máu tương thích có sẵn
            var compatibleTypes = GetCompatibleBloodTypes(bloodTypeString);
            foreach (var compatibleType in compatibleTypes)
            {
                var compatibleBloodType = (await _bloodTypeRepository.GetAllAsync())
                    .FirstOrDefault(bt => $"{bt.AboType}{bt.RhFactor}" == compatibleType);
                
                if (compatibleBloodType != null)
                {
                    var availableQuantity = await _bloodTypeRepository.GetAvailableUnitsQuantityAsync(compatibleBloodType.BloodTypeId);
                    totalAvailableQuantity += availableQuantity;
                    _logger.LogInformation(
                        "Compatible blood type {CompatibleType} has {Available}ml available",
                        compatibleType, availableQuantity);
                }
            }

            var initialStatus = totalAvailableQuantity >= quantityNeeded
                ? BloodRequestStatus.Pending.ToString()
                : BloodRequestStatus.Opened.ToString();

            // Tìm hoặc tạo recipient
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
            }

            // Tạo yêu cầu máu khẩn cấp
            var compatibleTypesStr = string.Join(", ", compatibleTypes);
            var bloodRequest = new BloodRequest
            {
                RequestId = Guid.NewGuid(),
                RecipientId = recipient.RecipientId,
                BloodTypeRequired = bloodTypeId,  // Sử dụng bloodTypeId đã xác định
                QuantityNeeded = request.QuantityNeeded,
                UrgencyLevel = "Emergency",
                RequestDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = initialStatus,
                Description = $"Emergency request. Required blood type: {bloodTypeString}. Compatible types: {compatibleTypesStr}. " +
                            $"Total available compatible blood: {totalAvailableQuantity}ml. {request.Description}"
            };

            await _requestRepository.AddAsync(bloodRequest);
            await _requestRepository.SaveChangesAsync();

            // Tạo thông báo
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = existingUser.UserId,
                NotificationType = "Emergency Blood Request",
                Message = initialStatus == BloodRequestStatus.Pending.ToString()
                    ? $"Emergency blood request has been created with sufficient compatible blood in inventory. Request ID: {bloodRequest.RequestId}"
                    : $"Emergency blood request has been created and opened for donors. Compatible blood types needed: {compatibleTypesStr}. Request ID: {bloodRequest.RequestId}",
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
    //private bool IsValidStatusTransition(BloodRequestStatus currentStatus, BloodRequestStatus newStatus)
    //{
    //    return (currentStatus, newStatus) switch
    //    {
    //        (BloodRequestStatus.Draft, BloodRequestStatus.Pending) => true,
    //        (BloodRequestStatus.Pending, BloodRequestStatus.Opened) => true,
    //        (BloodRequestStatus.Pending, BloodRequestStatus.Approved) => true,
    //        (BloodRequestStatus.Pending, BloodRequestStatus.Rejected) => true,
    //        (BloodRequestStatus.Opened, BloodRequestStatus.Approved) => true,
    //        (BloodRequestStatus.Opened, BloodRequestStatus.Rejected) => true,
    //        (BloodRequestStatus.Opened, BloodRequestStatus.Pending) => true,
    //        (BloodRequestStatus.Approved, BloodRequestStatus.Done) => true,
    //        (BloodRequestStatus.Approved, BloodRequestStatus.Closed) => true,
    //        (BloodRequestStatus.Done, BloodRequestStatus.Closed) => true,
            
    //        _ => false
    //    };
    //}
    public async Task<(bool success, string message)> UpdateRequestStatusAsync(
    Guid requestId,
    BloodRequestStatus newStatus,
    Guid userId)
    {
        try
        {
            // Kiểm tra request tồn tại
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return (false, "Blood request not found");
            }

            // Kiểm tra user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            // Parse current status 
            if (!Enum.TryParse<BloodRequestStatus>(request.Status, out var currentStatus))
            {
                return (false, "Invalid current status");
            }

            // Validate status transition dựa trên role
            var isValidTransition = IsValidStatusTransition(currentStatus, newStatus, user.Role);
            if (!isValidTransition)
            {
                return (false, $"Invalid status transition from {currentStatus} to {newStatus} for role {user.Role}");
            }

            // Update status
            request.Status = newStatus.ToString();

            // Create notification
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Blood Request Status Update",
                Message = $"Your blood request (ID: {requestId}) status has been updated to {newStatus} by {user.Role}",
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

    private bool IsValidStatusTransition(BloodRequestStatus currentStatus, BloodRequestStatus newStatus, string userRole)
    {
        // Staff có thể thực hiện mọi chuyển đổi trạng thái
        if (userRole == "Staff" || userRole == "Admin")
        {
            return (currentStatus, newStatus) switch
            {
                (BloodRequestStatus.Draft, BloodRequestStatus.Pending) => true,
                (BloodRequestStatus.Pending, BloodRequestStatus.Opened) => true,
                (BloodRequestStatus.Pending, BloodRequestStatus.Approved) => true,
                (BloodRequestStatus.Pending, BloodRequestStatus.Rejected) => true,
                (BloodRequestStatus.Pending, BloodRequestStatus.Done) => true,
                (BloodRequestStatus.Pending, BloodRequestStatus.Closed) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Approved) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Rejected) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Pending) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Done) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Closed) => true,
                (BloodRequestStatus.Approved, BloodRequestStatus.Done) => true,
                (BloodRequestStatus.Approved, BloodRequestStatus.Closed) => true,
                (BloodRequestStatus.Done, BloodRequestStatus.Closed) => true,
                _ => false
            };
        }
        
        // Member chỉ có thể thực hiện một số chuyển đổi giới hạn
        if (userRole == "Member")
        {
            return (currentStatus, newStatus) switch
            {
                // Member có thể chuyển từ Draft sang Pending
                (BloodRequestStatus.Draft, BloodRequestStatus.Pending) => true,
                
                // Member có thể chuyển từ Pending sang Opened hoặc ngược lại
                (BloodRequestStatus.Pending, BloodRequestStatus.Opened) => true,
                (BloodRequestStatus.Opened, BloodRequestStatus.Pending) => true,
                
                // Member có thể đóng request ở trạng thái Done
                (BloodRequestStatus.Done, BloodRequestStatus.Closed) => true,
                
                _ => false
            };
        }

        // Donor có thể thực hiện một số chuyển đổi khác
        if (userRole == "Donor") 
        {
            return (currentStatus, newStatus) switch
            {
                // Donor có thể chuyển từ Opened sang Pending
                (BloodRequestStatus.Opened, BloodRequestStatus.Pending) => true,
                
                _ => false
            };
        }

        // Mặc định không cho phép chuyển đổi
        return false;
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
    public async Task<UserDetailDTO> GetRequestUserDetailsAsync(Guid requestId)
    {
        try
        {
            // Lấy request với thông tin chi tiết (bao gồm Recipient và User)
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null || request.Recipient == null || request.Recipient.User == null)
            {
                return null;
            }

            // Map thông tin sang DTO
            var userDetails = new UserDetailDTO
            {
                UserId = request.Recipient.User.UserId,
                FullName = request.Recipient.User.FullName,
                Email = request.Recipient.User.Email,
                Phone = request.Recipient.User.Phone,
                UserIdCard = request.Recipient.User.UserIdCard,
                DateOfBirth = request.Recipient.User.DateOfBirth,
                Status = request.Status,
                Description = request.Description,
                QuantityNeeded = request.QuantityNeeded,
                RequestDate = request.RequestDate
            };

            return userDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user details for blood request {RequestId}", requestId);
            throw;
        }
    }
    private List<string> GetCompatibleBloodTypes(string bloodType)
    {
        // Return blood types that can donate to the specified blood type
        return bloodType.ToUpper() switch
        {
            "A+" => new List<string> { "A+", "A-", "O+", "O-" },
            "A-" => new List<string> { "A-", "O-" },
            "B+" => new List<string> { "B+", "B-", "O+", "O-" },
            "B-" => new List<string> { "B-", "O-" },
            "AB+" => new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" },
            "AB-" => new List<string> { "A-", "B-", "AB-", "O-" },
            "O+" => new List<string> { "O+", "O-" },
            "O-" => new List<string> { "O-" },
            _ => new List<string>()
        };
    }
}