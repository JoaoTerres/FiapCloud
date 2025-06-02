using System.Net;
using System.Threading.Tasks; 
using FiapCLoud.Domain.Exceptions; 
using FiapCLoud.Domain.Interfaces; 

namespace FiapCLoud.Domain.Services;

public class GameDomainService : IGameDomainService 
{
    private readonly IGameRepository _gameRepository;

    public GameDomainService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task ValidateGameNameIsUniqueAsync(string name)
    {
        var exists = await _gameRepository.ExistsByNameAsync(name); 
        if (exists)
            throw new DomainException($"Já existe um jogo com o nome '{name}'.", HttpStatusCode.BadRequest); 
    }
}