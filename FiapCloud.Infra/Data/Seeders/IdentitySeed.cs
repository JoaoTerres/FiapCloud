
using FiapCLoud.Domain.Entities; 
using FiapCLoud.Domain.Interfaces; 
using FiapCloud.Infra.Identity; 
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FiapCloud.Infra.Migrations.Seeders; 

public static class IdentitySeed
{
    public static async Task SeedRolesAndClaimsAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = { "Administrador", "Usuario" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        var adminRole = await roleManager.FindByNameAsync("Administrador");
        if (adminRole != null)
        {
            var adminClaims = new[]
            {
                new Claim("Permissao", "CadastrarJogos"),
                new Claim("Permissao", "AdministrarUsuarios"),
                new Claim("Permissao", "CriarPromocoes"),
                new Claim("Permissao", "AcessarPlataforma"), 
                new Claim("Permissao", "AcessarBiblioteca") 
            };

            foreach (var claim in adminClaims)
            {
                var currentClaims = await roleManager.GetClaimsAsync(adminRole);
                if (!currentClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await roleManager.AddClaimAsync(adminRole, claim);
                }
            }
        }

        var userRole = await roleManager.FindByNameAsync("Usuario");
        if (userRole != null)
        {
            var userClaims = new[]
            {
                new Claim("Permissao", "AcessarPlataforma"),
                new Claim("Permissao", "AcessarBiblioteca"),
            };

            foreach (var claim in userClaims)
            {
                var currentClaims = await roleManager.GetClaimsAsync(userRole);
                if (!currentClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await roleManager.AddClaimAsync(userRole, claim);
                }
            }
        }
    }

    public static async Task SeedUsersAsync(
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        RoleManager<IdentityRole<Guid>> roleManager) 
    {
        if (await userManager.FindByEmailAsync("admin@fiapcloud.com") == null)
        {
            var adminAppUser = new ApplicationUser
            {
                UserName = "admin@fiapcloud.com",
                Email = "admin@fiapcloud.com",
                EmailConfirmed = true
            };
            var adminResult = await userManager.CreateAsync(adminAppUser, "Admin@123!"); 
            if (adminResult.Succeeded)
            {
                if (await roleManager.RoleExistsAsync("Administrador"))
                {
                    await userManager.AddToRoleAsync(adminAppUser, "Administrador");
                }
                var adminDomainUser = new DomainUser("Administrador FiapCloud", adminAppUser.Id);
                await userRepository.AddAsync(adminDomainUser);
            }
            
        }

        
        if (await userManager.FindByEmailAsync("usuario@fiapcloud.com") == null)
        {
            var userAppUser = new ApplicationUser
            {
                UserName = "usuario@fiapcloud.com",
                Email = "usuario@fiapcloud.com",
                EmailConfirmed = true
            };
            var userResult = await userManager.CreateAsync(userAppUser, "Usuario@123!"); 
            if (userResult.Succeeded)
            {
                if (await roleManager.RoleExistsAsync("Usuario"))
                {
                    await userManager.AddToRoleAsync(userAppUser, "Usuario");
                }
                var userDomainUser = new DomainUser("Usuário FiapCloud", userAppUser.Id);
                await userRepository.AddAsync(userDomainUser);
            }
            
        }
        
        
        await userRepository.CommitAsync();
    }
}