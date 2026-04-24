using Tidy_BackendTest_MidLevel.Application.Interfaces;

namespace Tidy_BackendTest_MidLevel.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
        {
            tenantContext.TenantId = tenantId.ToString().Trim();
        }

        await _next(context);
    }
}
