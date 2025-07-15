using System;

namespace Models.DTOs
{
    public class BloodInventoryStatsDTO
    {
        public Dictionary<string, int> BloodTypeQuantities { get; set; }
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
        public int ExpiringSoonUnits { get; set; }
        public List<BloodUnitExpiryDTO> ExpiringUnits { get; set; }
    }

    public class BloodUnitExpiryDTO
    {
        public string BloodType { get; set; }
        public int Quantity { get; set; }
        public DateOnly ExpiryDate { get; set; }
    }

    public class DonationStatsDTO
    {
        public int TotalDonations { get; set; }
        public int CompletedDonations { get; set; }
        public int PendingDonations { get; set; }
        public Dictionary<string, int> DonationsByStatus { get; set; }
        public Dictionary<string, int> DonationsByBloodType { get; set; }
        public List<DailyDonationStatsDTO> DailyStats { get; set; }
    }

    public class DailyDonationStatsDTO
    {
        public DateOnly Date { get; set; }
        public int DonationCount { get; set; }
        public double TotalQuantity { get; set; }
    }

    public class RequestStatsDTO
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int FulfilledRequests { get; set; }
        public Dictionary<string, int> RequestsByStatus { get; set; }
        public Dictionary<string, int> RequestsByBloodType { get; set; }
        public List<EmergencyRequestStatsDTO> EmergencyRequests { get; set; }
    }

    public class EmergencyRequestStatsDTO
    {
        public DateOnly Date { get; set; }
        public int RequestCount { get; set; }
        public string BloodType { get; set; }
        public int QuantityNeeded { get; set; }
    }

    public class DonorStatsDTO
    {
        public int TotalDonors { get; set; }
        public int ActiveDonors { get; set; }
        public Dictionary<string, int> DonorsByBloodType { get; set; }
        public int NewDonorsThisMonth { get; set; }
        public List<TopDonorDTO> TopDonors { get; set; }
    }

    public class TopDonorDTO
    {
        public string DonorName { get; set; }
        public string BloodType { get; set; }
        public int TotalDonations { get; set; }
        public DateOnly LastDonationDate { get; set; }
    }

    public class DashboardSummaryDTO
    {
        public int TotalDonors { get; set; }
        public int TotalDonations { get; set; }
        public int PendingRequests { get; set; }
        public int AvailableBloodUnits { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; }
        public Dictionary<string, int> LowBloodTypeInventory { get; set; }
    }

    public class RecentActivityDTO
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public Guid? ReferenceId { get; set; }
    }
}