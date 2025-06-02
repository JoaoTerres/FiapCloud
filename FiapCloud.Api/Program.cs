using FIapCloud.App;
using FiapCloud.Infra;
using FiapCloud.Api.Middalware;
using FiapCloud.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using FiapCloud.Infra.Migrations.Seeders;
using FIapCloud.App.Services;
using FiapCLoud.Domain.Interfaces;
using FiapCloud.Infra.Identity;
using FiapCloud.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    options.IncludeScopes = true;
    options.SingleLine = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services
    .AddCustomIdentity(connectionString)
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomAuthorizationPolicies();


builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddAppServices();

builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FiapCloud API V1");
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<DomainExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await IdentitySeed.SeedRolesAndClaimsAsync(roleManager);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var userRepository = services.GetRequiredService<IUserRepository>();
        await IdentitySeed.SeedUsersAsync(userManager, userRepository, roleManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro crítico durante a semeadura dos dados iniciais (seed).");
    }
}

app.Run();