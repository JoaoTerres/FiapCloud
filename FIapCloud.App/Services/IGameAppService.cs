using FIapCloud.App.Dtos;

namespace FIapCloud.App.Services;

public interface IGameAppService
{
    Task<Guid> CreateGameAsync(CreateGameRequest request);
    Task<IEnumerable<GameResponse>> GetAllGamesAsync();
}

