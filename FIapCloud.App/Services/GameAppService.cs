
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using FiapCLoud.Domain.Services;
using FIapCloud.App.Dtos;
using Microsoft.Extensions.Logging;

namespace FIapCloud.App.Services;

public class GameAppService : IGameAppService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameDomainService _gameDomainService;
    private readonly ILogger<GameAppService> _logger;

    public GameAppService(
        IGameRepository gameRepository,
        IGameDomainService gameDomainService,
        ILogger<GameAppService> logger)
    {
        _gameRepository = gameRepository;
        _gameDomainService = gameDomainService;
        _logger = logger;
    }

    public async Task<Guid> CreateGameAsync(CreateGameRequest request)
    {
        _logger.LogInformation("AppService: Iniciando criação do jogo com nome: {GameName}, Preço: {GamePrice}", request.Name, request.Price);

        await _gameDomainService.ValidateGameNameIsUniqueAsync(request.Name);
        _logger.LogInformation("AppService: Validação de nome único para '{GameName}' concluída.", request.Name);

        var game = new Game(request.Name, request.Description, request.Price);
        game.Validate();
        _logger.LogInformation("AppService: Entidade Game criada com ID: {GameId}", game.Id);

        await _gameRepository.AddAsync(game);
        var commitSuccess = await _gameRepository.CommitAsync();

        if (!commitSuccess)
        {
            _logger.LogError("AppService: Falha ao persistir o novo jogo {GameName} (ID: {GameId}) no banco de dados.", game.Name, game.Id);

        } else
        {
            _logger.LogInformation("AppService: Jogo {GameName} (ID: {GameId}) criado e persistido com sucesso.", game.Name, game.Id);
        }
        
        return game.Id;
    }

    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
    {
        _logger.LogInformation("AppService: Buscando todos os jogos.");
        var games = await _gameRepository.GetAllAsync();

        if (games == null || !games.Any())
        {
            _logger.LogInformation("AppService: Nenhum jogo encontrado.");
            return Enumerable.Empty<GameResponse>();
        }

        var response = games.Select(game => new GameResponse
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            Price = game.Price,
            CreatedAt = game.CreatedAt
        }).ToList();
        _logger.LogInformation("AppService: Retornando {GameCount} jogos.", response.Count);
        return response;
    }
}