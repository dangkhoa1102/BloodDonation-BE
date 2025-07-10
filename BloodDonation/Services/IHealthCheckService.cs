using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models;

public interface IHealthCheckService
{
    Task<IEnumerable<HealthCheck>> GetAllAsync();
    Task<HealthCheck> GetByIdAsync(Guid id);
    Task<HealthCheck> AddAsync(HealthCheckDTO dto);

    Task UpdateAsync(Guid id, HealthCheckDTO dto);
    Task DeleteAsync(Guid id);
    Task<List<Guid>> GetAvailableDonorIdsAsync();

    Task ApproveHealthCheckAsync(Guid healthCheckId);
}
