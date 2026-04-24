using Tidy_BackendTest_MidLevel.Domain.Enums;

namespace Tidy_BackendTest_MidLevel.Domain.Entities;

public class Tenant
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
