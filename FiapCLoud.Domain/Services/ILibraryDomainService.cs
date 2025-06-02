using FiapCLoud.Domain.Entities;

namespace FiapCLoud.Domain.Services;

public interface ILibraryDomainService
{
    void EnsureGameIsNotInLibrary(DomainUser user, Game gameToAdd);
}
