using FIapCloud.App.Dtos;

namespace FIapCloud.App.Services;

public interface ILibraryAppService
{
    Task AddGameToUserLibraryAsync(Guid applicationUserId, Guid gameId);
    Task<IEnumerable<GameResponse>> GetGamesInUserLibraryAsync(Guid applicationUserId);
}
