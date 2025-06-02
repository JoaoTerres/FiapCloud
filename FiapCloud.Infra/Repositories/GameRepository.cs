
using FiapCloud.Infra.Context;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 

namespace FiapCloud.Infra.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<GameRepository> _logger; 

    public GameRepository(AppDbContext context, ILogger<GameRepository> logger) 
    {
        _context = context;
        _logger = logger; 
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        _logger.LogInformation("Verificando existência do jogo por nome: {GameName}", name);
        var exists = await _context.Games.AnyAsync(g => g.Name == name);
        _logger.LogInformation("Jogo com nome {GameName} {ExistsStatus}.", name, exists ? "encontrado" : "não encontrado");
        return exists;
    }

    public async Task AddAsync(Game game)
    {
        _logger.LogInformation("Adicionando novo jogo com ID: {GameId}, Nome: {GameName}", game.Id, game.Name);
        await _context.Games.AddAsync(game);
        _logger.LogInformation("Jogo com ID: {GameId} adicionado ao contexto.", game.Id);
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os jogos.");
        var games = await _context.Games.AsNoTracking().ToListAsync();
        _logger.LogInformation("Encontrados {GameCount} jogos.", games.Count);
        return games;
    }

    public async Task<Game?> GetByIdAsync(Guid gameId)
    {
        _logger.LogInformation("Buscando jogo por ID: {GameId}", gameId);
        var game = await _context.Games.FindAsync(gameId);
        if (game == null)
        {
            _logger.LogWarning("Jogo com ID: {GameId} não encontrado.", gameId);
        }
        else
        {
            _logger.LogInformation("Jogo com ID: {GameId} encontrado: {GameName}", gameId, game.Name);
        }
        return game;
    }

    public async Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> gameIds)
    {
        _logger.LogInformation("Buscando jogos por lista de IDs. Contagem de IDs: {IdCount}", gameIds.Count());
        if (gameIds == null || !gameIds.Any())
        {
            _logger.LogInformation("Nenhum ID fornecido para GetByIdsAsync.");
            return Enumerable.Empty<Game>();
        }
        var games = await _context.Games.Where(g => gameIds.Contains(g.Id)).ToListAsync();
        _logger.LogInformation("Encontrados {GameCount} jogos para a lista de IDs fornecida.", games.Count);
        return games;
    }

    public async Task<bool> CommitAsync()
    {
        _logger.LogInformation("Tentando persistir alterações no banco de dados para Games.");
        var result = await _context.SaveChangesAsync() > 0;
        _logger.LogInformation("Alterações para Games {StatusSalvo} com sucesso.", result ? "salvas" : "não salvas (ou nenhuma alteração detectada)");
        return result;
    }
}