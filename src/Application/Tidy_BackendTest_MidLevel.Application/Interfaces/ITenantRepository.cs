using Tidy_BackendTest_MidLevel.Domain.Entities;

namespace Tidy_BackendTest_MidLevel.Application.Interfaces;

public interface ITenantRepository
{
    Task<IEnumerable<Tenant>> GetAllAsync();
    Task<Tenant?> GetByIdAsync(string tenantId);
    Task<Tenant> AddAsync(Tenant tenant);
    Task<Tenant> UpdateAsync(Tenant tenant);
    Task<bool> DeleteAsync(string tenantId);
    Task<bool> ExistsAsync(string tenantId);
}
