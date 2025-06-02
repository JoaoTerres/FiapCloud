using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using FiapCLoud.Domain.Interfaces;
using FiapCLoud.Domain.Services;
using FIapCloud.App.Dtos;
using System.Net;

namespace FIapCloud.App.Services;

public class LibraryAppService : ILibraryAppService
{
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILibraryDomainService _libraryDomainService;

    public LibraryAppService(
        IUserRepository userRepository,
        IGameRepository gameRepository,
        ILibraryDomainService libraryDomainService)
    {
        _userRepository = userRepository;
        _gameRepository = gameRepository;
        _libraryDomainService = libraryDomainService;
    }

    public async Task AddGameToUserLibraryAsync(Guid applicationUserId, Guid gameId)
    {
        var domainUser = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
        {
            throw new DomainException("Usuário não encontrado.", HttpStatusCode.NotFound);
        }
        if (!domainUser.IsActive)
        {
            throw new DomainException("Usuário desativado. Não é possível adicionar jogos à biblioteca.", HttpStatusCode.Forbidden);
        }

        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            throw new DomainException("Jogo não encontrado.", HttpStatusCode.NotFound);
        }

        _libraryDomainService.EnsureGameIsNotInLibrary(domainUser, game);

        var sale = new Sale(game.Id, 1, game.Price, DateTime.UtcNow);
        sale.Validate();

        domainUser.Library.AddSale(sale);

        await _userRepository.CommitAsync();
    }

    public async Task<IEnumerable<GameResponse>> GetGamesInUserLibraryAsync(Guid applicationUserId)
    {
        var domainUser = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
        {
            throw new DomainException("Usuário não encontrado.", HttpStatusCode.NotFound);
        }

        if (domainUser.Library == null || !domainUser.Library.Sales.Any())
        {
            return Enumerable.Empty<GameResponse>();
        }

        var gameIds = domainUser.Library.Sales.Select(s => s.GameId).Distinct().ToList();
        if (!gameIds.Any())
        {
            return Enumerable.Empty<GameResponse>();
        }

        var gamesInLibraryDb = await _gameRepository.GetByIdsAsync(gameIds);

        var gameResponses = new List<GameResponse>();
        foreach (var game in gamesInLibraryDb)
        {
            gameResponses.Add(new GameResponse
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Price = game.Price,
                CreatedAt = game.CreatedAt
            });
        }
        return gameResponses;
    }
}