using FiapCloud.Infra.Repositories;
using FiapCLoud.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloud.Infra;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        return services;
    }
}