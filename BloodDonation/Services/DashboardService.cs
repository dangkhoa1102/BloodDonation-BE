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
        private readonly int LOW_INVENTORY_THRESHOLD = 10;
        private readonly int EXPIRY_WARNING_DAYS = 7;

        public DashboardService(
            BloodDonationSupportContext context,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BloodInventoryStatsDTO> GetBloodInventoryStatsAsync()
        {
            try
            {
                _logger.LogInformation("Starting to fetch blood inventory stats");
                
                var now = DateOnly.FromDateTime(DateTime.Now);
                var expiryThreshold = now.AddDays(EXPIRY_WARNING_DAYS);

                _logger.LogInformation("Fetching available blood units with expiry threshold: {ExpiryThreshold}", expiryThreshold);

                var bloodUnits = await _context.BloodUnits
                    .Include(b => b.BloodType)
                    .Where(b => b.Status == "available")
                    .ToListAsync();

                var stats = new BloodInventoryStatsDTO
                {
                    BloodTypeQuantities = GetBloodTypeQuantities(bloodUnits),
                    TotalUnits = bloodUnits.Sum(b => b.Quantity),
                    AvailableUnits = bloodUnits.Sum(b => b.Quantity),
                    ExpiringSoonUnits = GetExpiringSoonUnitsCount(bloodUnits, expiryThreshold),
                    ExpiringUnits = GetExpiringUnitsDetails(bloodUnits, expiryThreshold)
                };

                _logger.LogInformation("Successfully retrieved blood inventory stats. Total units: {TotalUnits}", stats.TotalUnits);
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
                var donations = await GetFilteredDonations(startDate, endDate);

                return new DonationStatsDTO
                {
                    TotalDonations = donations.Count,
                    CompletedDonations = GetCompletedDonationsCount(donations),
                    PendingDonations = GetPendingDonationsCount(donations),
                    DonationsByStatus = GetDonationsByStatus(donations),
                    DonationsByBloodType = GetDonationsByBloodType(donations),
                    DailyStats = GetDailyDonationStats(donations)
                };
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
                var requests = await GetFilteredRequests(startDate, endDate);

                return new RequestStatsDTO
                {
                    TotalRequests = requests.Count,
                    PendingRequests = GetPendingRequestsCount(requests),
                    FulfilledRequests = GetFulfilledRequestsCount(requests),
                    RequestsByStatus = GetRequestsByStatus(requests),
                    RequestsByBloodType = GetRequestsByBloodType(requests),
                    EmergencyRequests = GetEmergencyRequests(requests)
                };
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

                return new DonorStatsDTO
                {
                    TotalDonors = donors.Count,
                    ActiveDonors = GetActiveDonorsCount(donors, currentDate),
                    DonorsByBloodType = GetDonorsByBloodType(donors),
                    NewDonorsThisMonth = GetNewDonorsCount(donors, currentDate),
                    TopDonors = GetTopDonors(donors)
                };
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
                var summaryStats = await GetSummaryStats();
                var recentActivities = await GetRecentActivities();
                var lowInventory = await GetLowInventoryStats();

                return new DashboardSummaryDTO
                {
                    TotalDonors = summaryStats.TotalDonors,
                    TotalDonations = summaryStats.TotalDonations,
                    PendingRequests = summaryStats.PendingRequests,
                    AvailableBloodUnits = summaryStats.AvailableUnits,
                    RecentActivities = recentActivities,
                    LowBloodTypeInventory = lowInventory
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while getting dashboard summary");
                throw new InvalidOperationException("Failed to retrieve dashboard data due to database error", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                throw new InvalidOperationException("An unexpected error occurred while retrieving dashboard data", ex);
            }
        }

        #region Private Helper Methods

        private Dictionary<string, int> GetBloodTypeQuantities(List<BloodUnit> bloodUnits)
        {
            return bloodUnits
                .Where(b => b.BloodType != null)
                .GroupBy(b => $"{b.BloodType.AboType}{b.BloodType.RhFactor}")
                .ToDictionary(g => g.Key, g => g.Sum(b => b.Quantity));
        }

        private int GetExpiringSoonUnitsCount(List<BloodUnit> bloodUnits, DateOnly threshold)
        {
            return bloodUnits.Count(b => b.ExpiryDate <= threshold);
        }

        private List<BloodUnitExpiryDTO> GetExpiringUnitsDetails(List<BloodUnit> bloodUnits, DateOnly threshold)
        {
            return bloodUnits
                .Where(b => b.ExpiryDate <= threshold)
                .GroupBy(b => new { b.ExpiryDate, BloodType = $"{b.BloodType.AboType}{b.BloodType.RhFactor}" })
                .Select(g => new BloodUnitExpiryDTO
                {
                    BloodType = g.Key.BloodType,
                    ExpiryDate = g.Key.ExpiryDate ?? DateOnly.FromDateTime(DateTime.Now),
                    Quantity = g.Sum(b => b.Quantity)
                })
                .OrderBy(b => b.ExpiryDate)
                .ToList();
        }

        private async Task<List<BloodDonation>> GetFilteredDonations(DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.BloodDonations.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(d => d.DonationDate.HasValue &&
                    d.DonationDate.Value.Date >= startDate.Value.ToDateTime(TimeOnly.MinValue));

            if (endDate.HasValue)
                query = query.Where(d => d.DonationDate.HasValue &&
                    d.DonationDate.Value.Date <= endDate.Value.ToDateTime(TimeOnly.MinValue));

            return await query
                .Include(d => d.Donor)
                .ThenInclude(d => d.BloodType)
                .ToListAsync();
        }

        private int GetCompletedDonationsCount(List<BloodDonation> donations)
        {
            return donations.Count(d => d.Status == BloodDonationStatus.Completed.ToString());
        }

        private int GetPendingDonationsCount(List<BloodDonation> donations)
        {
            return donations.Count(d => d.Status == BloodDonationStatus.Pending.ToString());
        }

        private Dictionary<string, int> GetDonationsByStatus(List<BloodDonation> donations)
        {
            return donations
                .GroupBy(d => d.Status)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        private Dictionary<string, int> GetDonationsByBloodType(List<BloodDonation> donations)
        {
            return donations
                .Where(d => d.Donor?.BloodType != null)
                .GroupBy(d => $"{d.Donor.BloodType.AboType}{d.Donor.BloodType.RhFactor}")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private List<DailyDonationStatsDTO> GetDailyDonationStats(List<BloodDonation> donations)
        {
            return donations
                .Where(d => d.DonationDate.HasValue)
                .GroupBy(d => DateOnly.FromDateTime(d.DonationDate.Value))
                .Select(g => new DailyDonationStatsDTO
                {
                    Date = g.Key,
                    DonationCount = g.Count(),
                    TotalQuantity = g.Sum(d => d.Quantity ?? 0)
                })
                .OrderBy(s => s.Date)
                .ToList();
        }

        private async Task<List<BloodRequest>> GetFilteredRequests(DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.BloodRequests.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.RequestDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.RequestDate <= endDate.Value);

            return await query
                .Include(r => r.BloodTypeRequiredNavigation)
                .ToListAsync();
        }

        private List<EmergencyRequestStatsDTO> GetEmergencyRequests(List<BloodRequest> requests)
        {
            return requests
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
                .ToList();
        }

        private async Task<(int TotalDonors, int TotalDonations, int PendingRequests, int AvailableUnits)> GetSummaryStats()
        {
            return (
                await _context.Donors.CountAsync(),
                await _context.BloodDonations.CountAsync(),
                await _context.BloodRequests.CountAsync(r => r.Status == BloodRequestStatus.Pending.ToString()),
                await _context.BloodUnits.CountAsync(u => u.Status == "available")
            );
        }

        private async Task<List<RecentActivityDTO>> GetRecentActivities()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var recentActivities = new List<RecentActivityDTO>();

            // Get recent donations
            var recentDonations = await GetRecentDonations();
            recentActivities.AddRange(recentDonations);

            // Get recent requests
            var recentRequests = await GetRecentRequests();
            recentActivities.AddRange(recentRequests);

            return recentActivities.OrderByDescending(a => a.Date).Take(10).ToList();
        }

        private async Task<List<RecentActivityDTO>> GetRecentDonations()
        {
            try
            {
                return await _context.BloodDonations
                    .Include(d => d.Donor)
                    .ThenInclude(d => d.User)
                    .Where(d => d.Donor != null && d.Donor.User != null)  // Thêm điều kiện kiểm tra null
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .Select(d => new RecentActivityDTO
                    {
                        ActivityType = "Donation",
                        Description = $"Blood donation by {d.Donor.User.FullName}",
                        Date = d.DonationDate.HasValue
                            ? DateOnly.FromDateTime(d.DonationDate.Value)
                            : DateOnly.FromDateTime(DateTime.Now),
                        ReferenceId = d.DonationId
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent donations");
                return new List<RecentActivityDTO>();
            }
        }

        private async Task<List<RecentActivityDTO>> GetRecentRequests()
        {
            return await _context.BloodRequests
                .Include(r => r.Recipient)
                .ThenInclude(r => r.User)
                .OrderByDescending(r => r.RequestDate)
                .Take(5)
                .Select(r => new RecentActivityDTO
                {
                    ActivityType = "Request",
                    Description = $"Blood request from {r.Recipient.User.FullName}",
                    Date = r.RequestDate ?? DateOnly.FromDateTime(DateTime.Now),
                    ReferenceId = r.RequestId
                })
                .ToListAsync();
        }

        private async Task<Dictionary<string, int>> GetLowInventoryStats()
        {
            try
            {
                // Optimize query to get both blood type and quantity in one go
                var bloodUnitStats = await _context.BloodUnits
                    .Include(b => b.BloodType)
                    .Where(b => b.Status == "available" && b.BloodType != null)
                    .GroupBy(b => new { b.BloodType.AboType, b.BloodType.RhFactor })
                    .Select(g => new
                    {
                        BloodType = $"{g.Key.AboType}{g.Key.RhFactor}",
                        Count = g.Count()
                    })
                    .Where(x => x.Count < LOW_INVENTORY_THRESHOLD)
                    .ToDictionaryAsync(x => x.BloodType, x => x.Count);

                return bloodUnitStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low inventory stats");
                return new Dictionary<string, int>();
            }
        }

        private int GetActiveDonorsCount(List<Donor> donors, DateOnly currentDate)
        {
            return donors.Count(d => d.LastDonationDate >= currentDate.AddMonths(-3));
        }

        private Dictionary<string, int> GetDonorsByBloodType(List<Donor> donors)
        {
            return donors
                .Where(d => d.BloodType != null)
                .GroupBy(d => $"{d.BloodType.AboType}{d.BloodType.RhFactor}")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private int GetNewDonorsCount(List<Donor> donors, DateOnly currentDate)
        {
            var firstDayOfMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);
            return donors.Count(d => d.User?.DateOfBirth >= firstDayOfMonth);
        }

        private List<TopDonorDTO> GetTopDonors(List<Donor> donors)
        {
            return donors
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
                .ToList();
        }

        private int GetPendingRequestsCount(List<BloodRequest> requests)
        {
            return requests.Count(r => r.Status == BloodRequestStatus.Pending.ToString());
        }

        private int GetFulfilledRequestsCount(List<BloodRequest> requests)
        {
            return requests.Count(r => r.Status == BloodRequestStatus.Done.ToString());
        }

        private Dictionary<string, int> GetRequestsByStatus(List<BloodRequest> requests)
        {
            return requests
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        private Dictionary<string, int> GetRequestsByBloodType(List<BloodRequest> requests)
        {
            return requests
                .Where(r => r.BloodTypeRequiredNavigation != null)
                .GroupBy(r => $"{r.BloodTypeRequiredNavigation.AboType}{r.BloodTypeRequiredNavigation.RhFactor}")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        #endregion
    }
}