using Models.DTOs;

namespace Services
{
    public interface IDashboardService
    {
        Task<BloodInventoryStatsDTO> GetBloodInventoryStatsAsync();
        Task<DonationStatsDTO> GetDonationStatsAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<RequestStatsDTO> GetRequestStatsAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<DonorStatsDTO> GetDonorStatsAsync();
        Task<DashboardSummaryDTO> GetDashboardSummaryAsync();
    }
}