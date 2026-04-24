using System.ComponentModel.DataAnnotations;
using Tidy_BackendTest_MidLevel.Domain.Enums;

namespace Tidy_BackendTest_MidLevel.Application.DTOs;

public class UpdateTenantRequest
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(200)]
    public string? Domain { get; set; }

    public SubscriptionStatus? SubscriptionStatus { get; set; }
}
