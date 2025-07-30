using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services.BackgroundServices
{
    public class BloodUnitStatusUpdateService : BackgroundService
    {
        private readonly ILogger<BloodUnitStatusUpdateService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public BloodUnitStatusUpdateService(
            IServiceProvider serviceProvider,
            ILogger<BloodUnitStatusUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Blood unit status check starting at: {time}", DateTimeOffset.Now);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bloodUnitService = scope.ServiceProvider.GetRequiredService<IBloodUnitService>();
                        await bloodUnitService.UpdateExpiredUnitsAsync();
                    }

                    _logger.LogInformation("Blood unit status check completed at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating blood unit statuses");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}