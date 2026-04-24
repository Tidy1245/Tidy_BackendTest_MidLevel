using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Infrastructure.Data;
using Tidy_BackendTest_MidLevel.Infrastructure.Repositories;

namespace Tidy_BackendTest_MidLevel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(3)));

        services.AddScoped<IMyOfficeAcpdRepository, MyOfficeAcpdRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();

        return services;
    }
}
