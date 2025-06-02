namespace FiapCLoud.Domain.Services;

public interface IGameDomainService
{
    Task ValidateGameNameIsUniqueAsync(string name);
}