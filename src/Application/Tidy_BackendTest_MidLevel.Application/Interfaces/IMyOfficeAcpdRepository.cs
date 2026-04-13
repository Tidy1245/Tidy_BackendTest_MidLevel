using Tidy_BackendTest_MidLevel.Domain.Entities;

namespace Tidy_BackendTest_MidLevel.Application.Interfaces;

public interface IMyOfficeAcpdRepository
{
    Task<IEnumerable<MyOfficeAcpd>> GetAllAsync();
    Task<MyOfficeAcpd?> GetByIdAsync(string sid);
    Task<MyOfficeAcpd> AddAsync(MyOfficeAcpd entity);
    Task<MyOfficeAcpd> UpdateAsync(MyOfficeAcpd entity);
    Task<bool> DeleteAsync(string sid);
    Task<bool> ExistsAsync(string sid);
}
