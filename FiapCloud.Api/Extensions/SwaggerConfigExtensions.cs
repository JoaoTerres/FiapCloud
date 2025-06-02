using Microsoft.OpenApi.Models;

namespace FiapCloud.Api.Extensions;

public static class SwaggerConfigExtensions
{
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FiapCloud API",
                Version = "v1",
                Description = "API para a plataforma FiapCloud.",
                Contact = new OpenApiContact
                {
                    Name = "Seu Nome/Empresa",
                    Email = "seuemail@example.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Autenticação JWT usando o esquema Bearer. \r\n\r\n " +
                              "Entre com 'Bearer' [espaço] e então seu token no input de texto abaixo. \r\n\r\n" +
                              "Exemplo: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}