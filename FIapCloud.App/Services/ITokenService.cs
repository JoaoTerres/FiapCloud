using FIapCloud.App.Dtos;
using FiapCloud.Infra.Identity;

namespace FIapCloud.App.Services;

public interface ITokenService
{
    Task<LoginUserResponse> GenerateToken(ApplicationUser user);
}