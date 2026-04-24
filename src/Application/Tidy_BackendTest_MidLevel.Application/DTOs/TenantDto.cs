using Tidy_BackendTest_MidLevel.Domain.Enums;

namespace Tidy_BackendTest_MidLevel.Application.DTOs;

public class TenantDto
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
