using FIapCloud.App.Dtos;
using FiapCloud.Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FiapCLoud.Domain.Interfaces;

namespace FIapCloud.App.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public TokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _userRepository = userRepository;
        _roleManager = roleManager;
    }

    public async Task<LoginUserResponse> GenerateToken(ApplicationUser user)
    {
        var domainUser = await _userRepository.GetByApplicationUserIdAsync(user.Id);
        var userName = domainUser?.Name ?? user.UserName ?? "Usuário";

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, userName)
    };

        var userPrincipalClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userPrincipalClaims);

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            var identityRole = await _roleManager.FindByNameAsync(roleName);
            if (identityRole != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                foreach (var roleClaim in roleClaims)
                {
                    if (!claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value))
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
        }

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(_configuration["JwtSettings:ExpirationSeconds"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(token);

        return new LoginUserResponse
        {
            AccessToken = accessToken,
            ExpiresIn = (expires - DateTime.UtcNow).TotalSeconds,
            UserToken = new UserTokenResponse
            {
                Id = user.Id.ToString(),
                Email = user.Email!,
                Name = userName,
                Claims = claims.Select(c => new ClaimResponse { Type = c.Type, Value = c.Value }).ToList()
            }
        };
    }
}