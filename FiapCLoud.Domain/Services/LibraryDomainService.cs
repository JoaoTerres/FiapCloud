using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System.Net;

namespace FiapCLoud.Domain.Services;

public class LibraryDomainService : ILibraryDomainService
{
    public void EnsureGameIsNotInLibrary(DomainUser user, Game gameToAdd)
    {
        Validations.ValidateIfNull(user, "Usuário não pode ser nulo para verificar a biblioteca."); //
        Validations.ValidateIfNull(user.Library, "Biblioteca do usuário não pode ser nula para esta operação."); //
        Validations.ValidateIfNull(gameToAdd, "Jogo não pode ser nulo para verificar a biblioteca."); //

        bool gameExistsInLibrary = user.Library.Sales.Any(sale => sale.GameId == gameToAdd.Id);

        if (gameExistsInLibrary)
        {
            throw new DomainException($"O jogo '{gameToAdd.Name}' já existe na biblioteca do usuário '{user.Name}'.", HttpStatusCode.Conflict);
        }
    }
}