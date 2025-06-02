using Microsoft.Extensions.DependencyInjection;

namespace FiapCloud.Api.Extensions;


public static class AuthorizationPoliciesExtensions
{
    public static IServiceCollection AddCustomAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("PodeCadastrarJogos", policy => policy.RequireClaim("Permissao", "CadastrarJogos"));
            options.AddPolicy("PodeAdministrarUsuarios", policy => policy.RequireClaim("Permissao", "AdministrarUsuarios"));
            options.AddPolicy("PodeCriarPromocoes", policy => policy.RequireClaim("Permissao", "CriarPromocoes"));
            options.AddPolicy("PodeAcessarPlataforma", policy => policy.RequireClaim("Permissao", "AcessarPlataforma"));
            options.AddPolicy("PodeAcessarBiblioteca", policy => policy.RequireClaim("Permissao", "AcessarBiblioteca"));
        });

        return services;
    }
}
