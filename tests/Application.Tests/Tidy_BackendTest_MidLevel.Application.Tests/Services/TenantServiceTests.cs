using FluentAssertions;
using Moq;
using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Application.Services;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Domain.Enums;
using Tidy_BackendTest_MidLevel.Domain.Services;
using Xunit;

namespace Tidy_BackendTest_MidLevel.Application.Tests.Services;

public class TenantServiceTests
{
    private readonly Mock<ITenantRepository> _mockRepo;
    private readonly SidGeneratorService _sidGenerator;
    private readonly TenantService _service;

    public TenantServiceTests()
    {
        _mockRepo = new Mock<ITenantRepository>();
        _sidGenerator = new SidGeneratorService();
        _service = new TenantService(_mockRepo.Object, _sidGenerator);
    }

    [Fact]
    public async Task GetAllAsync_WhenTenantsExist_ReturnsDtoList()
    {
        var entities = new List<Tenant>
        {
            new() { TenantId = "ZA103ABCDE0000000001", Name = "租戶A", Domain = "a.com" },
            new() { TenantId = "ZA103ABCDE0000000002", Name = "租戶B", Domain = "b.com" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("租戶A");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tenant>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        const string tenantId = "ZA103ABCDE0000000001";
        var entity = new Tenant { TenantId = tenantId, Name = "租戶A", Domain = "a.com" };
        _mockRepo.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(tenantId);

        result.Should().NotBeNull();
        result!.TenantId.Should().Be(tenantId.Trim());
        result.Name.Should().Be("租戶A");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Tenant?)null);

        var result = await _service.GetByIdAsync("NOTEXIST0000000000XX");

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsAddAsyncOnce()
    {
        var request = new CreateTenantRequest { Name = "新租戶", Domain = "new.com" };
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Tenant>()))
            .ReturnsAsync((Tenant e) => e);

        await _service.CreateAsync(request);

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsDtoWithGeneratedTenantId()
    {
        var request = new CreateTenantRequest { Name = "新租戶" };
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Tenant>()))
            .ReturnsAsync((Tenant e) => e);

        var result = await _service.CreateAsync(request);

        result.TenantId.Should().HaveLength(20);
        result.Name.Should().Be("新租戶");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_SetsCreatedAt()
    {
        var request = new CreateTenantRequest { Name = "新租戶" };
        Tenant? captured = null;
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(e => captured = e)
            .ReturnsAsync((Tenant e) => e);

        await _service.CreateAsync(request);

        captured.Should().NotBeNull();
        captured!.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateAsync_DefaultSubscriptionStatus_IsTrial()
    {
        var request = new CreateTenantRequest { Name = "新租戶" };
        Tenant? captured = null;
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(e => captured = e)
            .ReturnsAsync((Tenant e) => e);

        await _service.CreateAsync(request);

        captured!.SubscriptionStatus.Should().Be(SubscriptionStatus.Trial);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_CallsUpdateAsyncOnce()
    {
        const string tenantId = "ZA103ABCDE0000000001";
        var entity = new Tenant { TenantId = tenantId, Name = "舊名稱" };
        _mockRepo.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(entity);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Tenant>()))
            .ReturnsAsync((Tenant e) => e);

        var request = new UpdateTenantRequest { Name = "新名稱" };
        await _service.UpdateAsync(tenantId, request);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Tenant>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Tenant?)null);

        var act = async () => await _service.UpdateAsync("NOTEXIST", new UpdateTenantRequest());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        const string tenantId = "ZA103ABCDE0000000001";
        var entity = new Tenant { TenantId = tenantId, Name = "原名稱", Domain = "original.com" };
        Tenant? captured = null;
        _mockRepo.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(entity);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(e => captured = e)
            .ReturnsAsync((Tenant e) => e);

        await _service.UpdateAsync(tenantId, new UpdateTenantRequest { Name = "新名稱" });

        captured!.Name.Should().Be("新名稱");
        captured.Domain.Should().Be("original.com");
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_CallsDeleteAsyncOnce()
    {
        const string tenantId = "ZA103ABCDE0000000001";
        _mockRepo.Setup(r => r.ExistsAsync(tenantId)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync(tenantId)).ReturnsAsync(true);

        await _service.DeleteAsync(tenantId);

        _mockRepo.Verify(r => r.DeleteAsync(tenantId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var act = async () => await _service.DeleteAsync("NOTEXIST");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
