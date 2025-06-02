namespace FiapCLoud.Domain.Services;

public interface IUserDomainService
{
    Task EnsureEmailIsUniqueAsync(string email);
    Task<bool> ActivateUserAsync(Guid userId);
    Task<bool> DeactivateUserAsync(Guid userId);
}
