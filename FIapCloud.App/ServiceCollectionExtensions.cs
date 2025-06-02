using FiapCLoud.Domain.Services;
using FIapCloud.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FIapCloud.App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IGameAppService, GameAppService>();
        services.AddScoped<IGameDomainService, GameDomainService>();
        services.AddScoped<IUserAppService, UserAppService>();
        services.AddScoped<IUserDomainService, UserDomainService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ILibraryAppService, LibraryAppService>();
        services.AddScoped<ILibraryDomainService, LibraryDomainService>();
        services.AddScoped<IPromotionAppService, PromotionAppService>();
        services.AddScoped<IPromotionDomainService, PromotionDomainService>();
        return services;
    }
}