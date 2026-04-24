using Tidy_BackendTest_MidLevel.Application.Interfaces;

namespace Tidy_BackendTest_MidLevel.Infrastructure.Data;

public class TenantContext : ITenantContext
{
    public string? TenantId { get; set; }
}
