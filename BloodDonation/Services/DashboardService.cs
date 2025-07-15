using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Enums;
using Repositories.Interfaces;
using Services.Interfaces;
using System;

namespace Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly BloodDonationSupportContext _context;
        private readonly ILogger<DashboardService> _logger;
        private readonly IBloodUnitRepository _bloodUnitRepo;
        private readonly IBloodDonationRepository _donationRepo;
        private readonly IBloodRequestRepository _requestRepo;
        private readonly IDonorRepository _donorRepo;

        public DashboardService(
            BloodDonationSupportContext context,
            ILogger<DashboardService> logger,
            IBloodUnitRepository bloodUnitRepo,
            IBloodDonationRepository donationRepo,
            IBloodRequestRepository requestRepo,
            IDonorRepository donorRepo)
        {
            _context = context;
            _logger = logger;
            _bloodUnitRepo = bloodUnitRepo;
            _donationRepo = donationRepo;
            _requestRepo = requestRepo;
            _donorRepo = donorRepo;
        }

        public async Task<BloodInventoryStatsDTO> GetBloodInventoryStatsAsync()
        {
            try
            {
                var bloodUnits = await _context.BloodUnits
                    .Include(b => b.BloodType)
                    .Where(b => b.Status == "available")
                    .ToListAsync();

                var now = DateOnly.FromDateTime(DateTime.Now);
                var expiryThreshold = now.AddDays(7); // Cảnh báo 7 ngày trước khi hết hạn

                var stats = new BloodInventoryStatsDTO
                {
                    BloodTypeQuantities = bloodUnits
                        .GroupBy(b => $"{b.BloodType.AboType}{b.BloodType.RhFactor}")
                        .ToDictionary(g => g.Key, g => g.Sum(b => b.Quantity)),
                    TotalUnits = bloodUnits.Count,
                    AvailableUnits = bloodUnits.Count(b => b.Status == "available"),
                    ExpiringSoonUnits = bloodUnits.Count(b => b.ExpiryDate <= expiryThreshold),
                    ExpiringUnits = bloodUnits
                        .Where(b => b.ExpiryDate <= expiryThreshold)
                        .GroupBy(b => new { b.ExpiryDate, BloodType = $"{b.BloodType.AboType}{b.BloodType.RhFactor}" })
                        .Select(g => new BloodUnitExpiryDTO
                        {
                            BloodType = g.Key.BloodType,
                            ExpiryDate = g.Key.ExpiryDate ?? DateOnly.FromDateTime(DateTime.Now),
                            Quantity = g.Sum(b => b.Quantity)
                        })
                        .OrderBy(b => b.ExpiryDate)
                        .ToList()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blood inventory stats");
                throw;
            }
        }

        public async Task<DonationStatsDTO> GetDonationStatsAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            try
            {
                var query = _context.BloodDonations.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(d => d.DonationDate.HasValue && d.DonationDate.Value.Date >= startDate.Value.ToDateTime(TimeOnly.MinValue).Date);
                if (endDate.HasValue)
                    query = query.Where(d => d.DonationDate.HasValue && d.DonationDate.Value.Date <= endDate.Value.ToDateTime(TimeOnly.MinValue).Date);

                var donations = await query
                    .Include(d => d.Donor)
                    .ThenInclude(d => d.BloodType)
                    .ToListAsync();

                var stats = new DonationStatsDTO
                {
                    TotalDonations = donations.Count,
                    CompletedDonations = donations.Count(d => d.Status == BloodDonationStatus.Completed.ToString()),
                    PendingDonations = donations.Count(d => d.Status == BloodDonationStatus.Pending.ToString()),
                    DonationsByStatus = donations
                        .GroupBy(d => d.Status)
                        .ToDictionary(g => g.Key ?? "Unknown", g => g.Count()),
                    DonationsByBloodType = donations
                        .Where(d => d.Donor?.BloodType != null)
                        .GroupBy(d => $"{d.Donor.BloodType.AboType}{d.Donor.BloodType.RhFactor}")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    DailyStats = donations
                    .Where(d => d.DonationDate.HasValue)
                    .GroupBy(d => DateOnly.FromDateTime(d.DonationDate.Value))
    .Select(g => new DailyDonationStatsDTO
    {
        Date = g.Key,
        DonationCount = g.Count(),
        TotalQuantity = g.Sum(d => d.Quantity ?? 0)
    })
    .OrderBy(s => s.Date)
    .ToList()

                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation stats");
                throw;
            }
        }

        public async Task<RequestStatsDTO> GetRequestStatsAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            try
            {
                var query = _context.BloodRequests.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(r => r.RequestDate >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(r => r.RequestDate <= endDate.Value);

                var requests = await query
                    .Include(r => r.BloodTypeRequiredNavigation)
                    .ToListAsync();

                var stats = new RequestStatsDTO
                {
                    TotalRequests = requests.Count,
                    PendingRequests = requests.Count(r => r.Status == BloodRequestStatus.Pending.ToString()),
                    FulfilledRequests = requests.Count(r => r.Status == BloodRequestStatus.Done.ToString()),
                    RequestsByStatus = requests
                        .GroupBy(r => r.Status)
                        .ToDictionary(g => g.Key ?? "Unknown", g => g.Count()),
                    RequestsByBloodType = requests
                        .Where(r => r.BloodTypeRequiredNavigation != null)
                        .GroupBy(r => $"{r.BloodTypeRequiredNavigation.AboType}{r.BloodTypeRequiredNavigation.RhFactor}")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    EmergencyRequests = requests
                        .Where(r => r.UrgencyLevel == "Emergency")
                        .Select(r => new EmergencyRequestStatsDTO
                        {
                            Date = r.RequestDate ?? DateOnly.FromDateTime(DateTime.Now),
                            RequestCount = 1,
                            BloodType = r.BloodTypeRequiredNavigation != null
                                ? $"{r.BloodTypeRequiredNavigation.AboType}{r.BloodTypeRequiredNavigation.RhFactor}"
                                : "Unknown",
                            QuantityNeeded = r.QuantityNeeded ?? 0
                        })
                        .OrderByDescending(r => r.Date)
                        .ToList()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting request stats");
                throw;
            }
        }

        public async Task<DonorStatsDTO> GetDonorStatsAsync()
        {
            try
            {
                var donors = await _context.Donors
                    .Include(d => d.BloodType)
                    .Include(d => d.DonationHistories)
                    .Include(d => d.User)
                    .ToListAsync();

                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var firstDayOfMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);

                var stats = new DonorStatsDTO
                {
                    TotalDonors = donors.Count,
                    ActiveDonors = donors.Count(d => d.LastDonationDate >= currentDate.AddMonths(-3)),
                    DonorsByBloodType = donors
                        .Where(d => d.BloodType != null)
                        .GroupBy(d => $"{d.BloodType.AboType}{d.BloodType.RhFactor}")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    NewDonorsThisMonth = donors.Count(d => d.User?.DateOfBirth >= firstDayOfMonth),
                    TopDonors = donors
                        .OrderByDescending(d => d.DonationHistories.Count)
                        .Take(10)
                        .Select(d => new TopDonorDTO
                        {
                            DonorName = d.User?.FullName ?? "Unknown",
                            BloodType = d.BloodType != null
                                ? $"{d.BloodType.AboType}{d.BloodType.RhFactor}"
                                : "Unknown",
                            TotalDonations = d.DonationHistories.Count,
                            LastDonationDate = d.LastDonationDate ?? DateOnly.FromDateTime(DateTime.Now)
                        })
                        .ToList()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donor stats");
                throw;
            }
        }

        public async Task<DashboardSummaryDTO> GetDashboardSummaryAsync()
        {
            try
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                // Lấy thông tin tổng quan
                var totalDonors = await _context.Donors.CountAsync();
                var totalDonations = await _context.BloodDonations.CountAsync();
                var pendingRequests = await _context.BloodRequests
                    .CountAsync(r => r.Status == BloodRequestStatus.Pending.ToString());
                var availableUnits = await _context.BloodUnits
                    .CountAsync(u => u.Status == "available");

                // Lấy các hoạt động gần đây
                var recentActivities = new List<RecentActivityDTO>();

                // Thêm hoạt động hiến máu gần đây
                var recentDonations = await _context.BloodDonations
                    .Include(d => d.Donor)
                    .ThenInclude(d => d.User)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .Select(d => new RecentActivityDTO
                    {
                        ActivityType = "Donation",
                        Description = $"Blood donation by {d.Donor.User.FullName}",
                        Date = d.DonationDate.HasValue 
            ? DateOnly.FromDateTime(d.DonationDate.Value) 
            : currentDate,
                        ReferenceId = d.DonationId
                    })
                    .ToListAsync();

                recentActivities.AddRange(recentDonations);

                // Thêm yêu cầu máu gần đây
                var recentRequests = await _context.BloodRequests
                    .Include(r => r.Recipient)
                    .ThenInclude(r => r.User)
                    .OrderByDescending(r => r.RequestDate)
                    .Take(5)
                    .Select(r => new RecentActivityDTO
                    {
                        ActivityType = "Request",
                        Description = $"Blood request from {r.Recipient.User.FullName}",
                        Date = r.RequestDate ?? currentDate,
                        ReferenceId = r.RequestId
                    })
                    .ToListAsync();

                recentActivities.AddRange(recentRequests);

                // Kiểm tra tồn kho thấp - Sửa lại phần này để tránh lỗi string.Format
                var bloodUnits = await _context.BloodUnits
                    .Include(b => b.BloodType)
                    .Where(b => b.Status == "available")
                    .ToListAsync();

                var lowInventory = bloodUnits
                    .GroupBy(b => b.BloodType != null ? $"{b.BloodType.AboType}{b.BloodType.RhFactor}" : "Unknown")
                    .Select(g => new { BloodType = g.Key, Count = g.Count() })
                    .Where(x => x.Count < 10)
                    .ToDictionary(x => x.BloodType, x => x.Count);

                return new DashboardSummaryDTO
                {
                    TotalDonors = totalDonors,
                    TotalDonations = totalDonations,
                    PendingRequests = pendingRequests,
                    AvailableBloodUnits = availableUnits,
                    RecentActivities = recentActivities.OrderByDescending(a => a.Date).Take(10).ToList(),
                    LowBloodTypeInventory = lowInventory
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                throw;
            }
        }
    }
}