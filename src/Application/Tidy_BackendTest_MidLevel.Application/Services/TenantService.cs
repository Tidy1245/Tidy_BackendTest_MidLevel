using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Domain.Services;

namespace Tidy_BackendTest_MidLevel.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _repository;
    private readonly SidGeneratorService _sidGenerator;

    public TenantService(ITenantRepository repository, SidGeneratorService sidGenerator)
    {
        _repository = repository;
        _sidGenerator = sidGenerator;
    }

    public async Task<IEnumerable<TenantDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<TenantDto?> GetByIdAsync(string tenantId)
    {
        var entity = await _repository.GetByIdAsync(tenantId);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<TenantDto> CreateAsync(CreateTenantRequest request)
    {
        string tenantId = _sidGenerator.Generate();

        int retries = 0;
        while (await _repository.ExistsAsync(tenantId) && retries < 3)
        {
            tenantId = _sidGenerator.Generate();
            retries++;
        }

        var entity = new Tenant
        {
            TenantId = tenantId,
            Name = request.Name,
            Domain = request.Domain,
            SubscriptionStatus = request.SubscriptionStatus,
            CreatedAt = DateTime.Now
        };

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<TenantDto> UpdateAsync(string tenantId, UpdateTenantRequest request)
    {
        var entity = await _repository.GetByIdAsync(tenantId)
            ?? throw new KeyNotFoundException($"TenantId '{tenantId}' 不存在");

        if (request.Name is not null) entity.Name = request.Name;
        if (request.Domain is not null) entity.Domain = request.Domain;
        if (request.SubscriptionStatus is not null) entity.SubscriptionStatus = request.SubscriptionStatus.Value;

        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(string tenantId)
    {
        var exists = await _repository.ExistsAsync(tenantId);
        if (!exists)
            throw new KeyNotFoundException($"TenantId '{tenantId}' 不存在");

        await _repository.DeleteAsync(tenantId);
    }

    private static TenantDto MapToDto(Tenant entity) => new()
    {
        TenantId = entity.TenantId.Trim(),
        Name = entity.Name,
        Domain = entity.Domain,
        SubscriptionStatus = entity.SubscriptionStatus,
        CreatedAt = entity.CreatedAt
    };
}
