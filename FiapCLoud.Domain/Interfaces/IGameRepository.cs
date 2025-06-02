using FiapCLoud.Domain.Entities;

namespace FiapCLoud.Domain.Interfaces;

public interface IGameRepository : IRepository<Game>
{
    Task<bool> ExistsByNameAsync(string name);
    Task AddAsync(Game game);
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid gameId);
    Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> gameIds);
}
