using FiapCloud.Infra.Context;
using FiapCloud.Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace FiapCloud.Api.Extensions;

public static class IdentityConfigExtensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


        return services;
    }
}
