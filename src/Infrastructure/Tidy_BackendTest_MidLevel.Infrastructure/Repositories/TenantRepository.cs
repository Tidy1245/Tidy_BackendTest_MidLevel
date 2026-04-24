using Microsoft.EntityFrameworkCore;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Infrastructure.Data;

namespace Tidy_BackendTest_MidLevel.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync()
    {
        return await _context.Tenants
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Tenant?> GetByIdAsync(string tenantId)
    {
        return await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.TenantId == tenantId.Trim());
    }

    public async Task<Tenant> AddAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }

    public async Task<bool> DeleteAsync(string tenantId)
    {
        var entity = await _context.Tenants
            .FirstOrDefaultAsync(e => e.TenantId == tenantId.Trim());

        if (entity is null) return false;

        _context.Tenants.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string tenantId)
    {
        return await _context.Tenants
            .AsNoTracking()
            .AnyAsync(e => e.TenantId == tenantId.Trim());
    }
}
