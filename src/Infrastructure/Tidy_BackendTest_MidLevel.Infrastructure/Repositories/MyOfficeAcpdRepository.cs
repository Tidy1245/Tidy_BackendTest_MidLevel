using Microsoft.EntityFrameworkCore;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Infrastructure.Data;

namespace Tidy_BackendTest_MidLevel.Infrastructure.Repositories;

public class MyOfficeAcpdRepository : IMyOfficeAcpdRepository
{
    private readonly AppDbContext _context;

    public MyOfficeAcpdRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MyOfficeAcpd>> GetAllAsync()
    {
        return await _context.MyOfficeAcpds
            .AsNoTracking()
            .OrderByDescending(e => e.ACPD_NowDateTime)
            .ToListAsync();
    }

    public async Task<MyOfficeAcpd?> GetByIdAsync(string sid)
    {
        // char(20) in SQL Server is right-padded with spaces; use Trim to avoid equality bugs
        return await _context.MyOfficeAcpds
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ACPD_SID == sid.Trim());
    }

    public async Task<MyOfficeAcpd> AddAsync(MyOfficeAcpd entity)
    {
        _context.MyOfficeAcpds.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<MyOfficeAcpd> UpdateAsync(MyOfficeAcpd entity)
    {
        _context.MyOfficeAcpds.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(string sid)
    {
        var entity = await _context.MyOfficeAcpds
            .FirstOrDefaultAsync(e => e.ACPD_SID == sid.Trim());

        if (entity is null) return false;

        _context.MyOfficeAcpds.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string sid)
    {
        return await _context.MyOfficeAcpds
            .AsNoTracking()
            .AnyAsync(e => e.ACPD_SID == sid.Trim());
    }
}
