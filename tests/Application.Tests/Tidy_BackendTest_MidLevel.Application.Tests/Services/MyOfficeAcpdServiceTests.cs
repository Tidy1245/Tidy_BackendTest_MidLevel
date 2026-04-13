using FluentAssertions;
using Moq;
using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Application.Services;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Domain.Services;
using Xunit;

namespace Tidy_BackendTest_MidLevel.Application.Tests.Services;

public class MyOfficeAcpdServiceTests
{
    private readonly Mock<IMyOfficeAcpdRepository> _mockRepo;
    private readonly SidGeneratorService _sidGenerator;
    private readonly MyOfficeAcpdService _service;

    public MyOfficeAcpdServiceTests()
    {
        _mockRepo = new Mock<IMyOfficeAcpdRepository>();
        _sidGenerator = new SidGeneratorService();
        _service = new MyOfficeAcpdService(_mockRepo.Object, _sidGenerator);
    }

    [Fact]
    public async Task GetAllAsync_WhenRecordsExist_ReturnsDtoList()
    {
        var entities = new List<MyOfficeAcpd>
        {
            new() { ACPD_SID = "ZA103ABCDE0000000001", ACPD_Cname = "測試一", ACPD_LoginID = "user01" },
            new() { ACPD_SID = "ZA103ABCDE0000000002", ACPD_Cname = "測試二", ACPD_LoginID = "user02" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().ACPD_Cname.Should().Be("測試一");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<MyOfficeAcpd>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecordExists_ReturnsDto()
    {
        const string sid = "ZA103ABCDE0000000001";
        var entity = new MyOfficeAcpd { ACPD_SID = sid, ACPD_Cname = "測試", ACPD_LoginID = "user01" };
        _mockRepo.Setup(r => r.GetByIdAsync(sid)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(sid);

        result.Should().NotBeNull();
        result!.ACPD_SID.Should().Be(sid.Trim());
        result.ACPD_Cname.Should().Be("測試");
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecordNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((MyOfficeAcpd?)null);

        var result = await _service.GetByIdAsync("NOTEXIST00000000000X");

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsAddAsyncOnce()
    {
        var request = new CreateMyOfficeAcpdRequest
        {
            ACPD_Cname = "新使用者",
            ACPD_LoginID = "newuser",
            ACPD_LoginPWD = "P@ssw0rd"
        };
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<MyOfficeAcpd>()))
            .ReturnsAsync((MyOfficeAcpd e) => e);

        await _service.CreateAsync(request);

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<MyOfficeAcpd>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsDtoWithGeneratedSid()
    {
        var request = new CreateMyOfficeAcpdRequest
        {
            ACPD_Cname = "新使用者",
            ACPD_LoginID = "newuser",
            ACPD_LoginPWD = "P@ssw0rd"
        };
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<MyOfficeAcpd>()))
            .ReturnsAsync((MyOfficeAcpd e) => e);

        var result = await _service.CreateAsync(request);

        result.ACPD_SID.Should().HaveLength(20);
        result.ACPD_Cname.Should().Be("新使用者");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_SetsAuditFields()
    {
        var request = new CreateMyOfficeAcpdRequest
        {
            ACPD_LoginID = "user01",
            ACPD_LoginPWD = "P@ssw0rd"
        };
        MyOfficeAcpd? captured = null;
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<MyOfficeAcpd>()))
            .Callback<MyOfficeAcpd>(e => captured = e)
            .ReturnsAsync((MyOfficeAcpd e) => e);

        await _service.CreateAsync(request, operatorId: "TESTUSER");

        captured.Should().NotBeNull();
        captured!.ACPD_NowID.Should().Be("TESTUSER");
        captured.ACPD_NowDateTime.Should().NotBeNull();
        captured.ACPD_UPDDateTime.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenRecordExists_CallsUpdateAsyncOnce()
    {
        const string sid = "ZA103ABCDE0000000001";
        var entity = new MyOfficeAcpd { ACPD_SID = sid, ACPD_Cname = "舊名", ACPD_LoginID = "user01" };
        _mockRepo.Setup(r => r.GetByIdAsync(sid)).ReturnsAsync(entity);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<MyOfficeAcpd>()))
            .ReturnsAsync((MyOfficeAcpd e) => e);

        var request = new UpdateMyOfficeAcpdRequest { ACPD_Cname = "新名" };
        await _service.UpdateAsync(sid, request);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<MyOfficeAcpd>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRecordNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((MyOfficeAcpd?)null);

        var act = async () => await _service.UpdateAsync("NOTEXIST", new UpdateMyOfficeAcpdRequest());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdatedAuditFields()
    {
        const string sid = "ZA103ABCDE0000000001";
        var entity = new MyOfficeAcpd { ACPD_SID = sid, ACPD_LoginID = "user01" };
        MyOfficeAcpd? captured = null;
        _mockRepo.Setup(r => r.GetByIdAsync(sid)).ReturnsAsync(entity);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<MyOfficeAcpd>()))
            .Callback<MyOfficeAcpd>(e => captured = e)
            .ReturnsAsync((MyOfficeAcpd e) => e);

        await _service.UpdateAsync(sid, new UpdateMyOfficeAcpdRequest { ACPD_Cname = "更新" }, operatorId: "ADMIN");

        captured!.ACPD_UPDID.Should().Be("ADMIN");
        captured.ACPD_UPDDateTime.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenRecordExists_CallsDeleteAsyncOnce()
    {
        const string sid = "ZA103ABCDE0000000001";
        _mockRepo.Setup(r => r.ExistsAsync(sid)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync(sid)).ReturnsAsync(true);

        await _service.DeleteAsync(sid);

        _mockRepo.Verify(r => r.DeleteAsync(sid), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRecordNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var act = async () => await _service.DeleteAsync("NOTEXIST");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
