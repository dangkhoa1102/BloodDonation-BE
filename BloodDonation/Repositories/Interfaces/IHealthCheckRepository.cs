using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories.Interfaces
{
    public interface IHealthCheckRepository
    {
        Task<IEnumerable<HealthCheck>> GetAllAsync();
        Task<HealthCheck> GetByIdAsync(Guid id);
        Task<HealthCheck> AddAsync(HealthCheck healthCheck);
        Task UpdateAsync(HealthCheck healthCheck);
        Task DeleteAsync(Guid id);
        Task<List<Guid>> GetAvailableDonorIdsAsync();
    }
}
