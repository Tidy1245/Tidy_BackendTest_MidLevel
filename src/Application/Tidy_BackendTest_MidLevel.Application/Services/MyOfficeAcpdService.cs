using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Domain.Services;

namespace Tidy_BackendTest_MidLevel.Application.Services;

public class MyOfficeAcpdService
{
    private readonly IMyOfficeAcpdRepository _repository;
    private readonly SidGeneratorService _sidGenerator;

    public MyOfficeAcpdService(IMyOfficeAcpdRepository repository, SidGeneratorService sidGenerator)
    {
        _repository = repository;
        _sidGenerator = sidGenerator;
    }

    public async Task<IEnumerable<MyOfficeAcpdDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<MyOfficeAcpdDto?> GetByIdAsync(string sid)
    {
        var entity = await _repository.GetByIdAsync(sid);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<MyOfficeAcpdDto> CreateAsync(CreateMyOfficeAcpdRequest request, string operatorId = "SYSTEM")
    {
        string sid = _sidGenerator.Generate();

        // Collision check with retry (max 3 attempts)
        int retries = 0;
        while (await _repository.ExistsAsync(sid) && retries < 3)
        {
            sid = _sidGenerator.Generate();
            retries++;
        }

        var entity = new MyOfficeAcpd
        {
            ACPD_SID = sid,
            ACPD_Cname = request.ACPD_Cname,
            ACPD_Ename = request.ACPD_Ename,
            ACPD_Sname = request.ACPD_Sname,
            ACPD_Email = request.ACPD_Email,
            ACPD_Status = request.ACPD_Status ?? 0,
            ACPD_Stop = request.ACPD_Stop ?? false,
            ACPD_StopMemo = request.ACPD_StopMemo,
            ACPD_LoginID = request.ACPD_LoginID,
            ACPD_LoginPWD = request.ACPD_LoginPWD,
            ACPD_Memo = request.ACPD_Memo,
            ACPD_NowDateTime = DateTime.Now,
            ACPD_NowID = operatorId,
            ACPD_UPDDateTime = DateTime.Now,
            ACPD_UPDID = operatorId
        };

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<MyOfficeAcpdDto> UpdateAsync(string sid, UpdateMyOfficeAcpdRequest request, string operatorId = "SYSTEM")
    {
        var entity = await _repository.GetByIdAsync(sid)
            ?? throw new KeyNotFoundException($"ACPD_SID '{sid}' 不存在");

        if (request.ACPD_Cname is not null) entity.ACPD_Cname = request.ACPD_Cname;
        if (request.ACPD_Ename is not null) entity.ACPD_Ename = request.ACPD_Ename;
        if (request.ACPD_Sname is not null) entity.ACPD_Sname = request.ACPD_Sname;
        if (request.ACPD_Email is not null) entity.ACPD_Email = request.ACPD_Email;
        if (request.ACPD_Status is not null) entity.ACPD_Status = request.ACPD_Status;
        if (request.ACPD_Stop is not null) entity.ACPD_Stop = request.ACPD_Stop;
        if (request.ACPD_StopMemo is not null) entity.ACPD_StopMemo = request.ACPD_StopMemo;
        if (request.ACPD_LoginID is not null) entity.ACPD_LoginID = request.ACPD_LoginID;
        if (request.ACPD_LoginPWD is not null) entity.ACPD_LoginPWD = request.ACPD_LoginPWD;
        if (request.ACPD_Memo is not null) entity.ACPD_Memo = request.ACPD_Memo;

        entity.ACPD_UPDDateTime = DateTime.Now;
        entity.ACPD_UPDID = operatorId;

        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(string sid)
    {
        var exists = await _repository.ExistsAsync(sid);
        if (!exists)
            throw new KeyNotFoundException($"ACPD_SID '{sid}' 不存在");

        await _repository.DeleteAsync(sid);
    }

    private static MyOfficeAcpdDto MapToDto(MyOfficeAcpd entity) => new()
    {
        ACPD_SID = entity.ACPD_SID.Trim(),
        ACPD_Cname = entity.ACPD_Cname,
        ACPD_Ename = entity.ACPD_Ename,
        ACPD_Sname = entity.ACPD_Sname,
        ACPD_Email = entity.ACPD_Email,
        ACPD_Status = entity.ACPD_Status,
        ACPD_Stop = entity.ACPD_Stop,
        ACPD_StopMemo = entity.ACPD_StopMemo,
        ACPD_LoginID = entity.ACPD_LoginID,
        ACPD_LoginPWD = entity.ACPD_LoginPWD,
        ACPD_Memo = entity.ACPD_Memo,
        ACPD_NowDateTime = entity.ACPD_NowDateTime,
        ACPD_NowID = entity.ACPD_NowID,
        ACPD_UPDDateTime = entity.ACPD_UPDDateTime,
        ACPD_UPDID = entity.ACPD_UPDID
    };
}
