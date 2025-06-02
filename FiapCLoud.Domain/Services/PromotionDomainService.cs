using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using FiapCLoud.Domain.Interfaces;
using System.Net;

namespace FiapCLoud.Domain.Services;

public class PromotionDomainService : IPromotionDomainService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;

    public PromotionDomainService(IGameRepository gameRepository, IPromotionRepository promotionRepository)
    {
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
    }

    public void ValidatePromotionRules(Promotion promotion)
    {
        promotion.Validate(); 
    }

    public async Task EnsureGamesExistAsync(IEnumerable<Guid> gameIds)
    {
        var distinctGameIds = gameIds.Distinct().ToList();
        var games = await _gameRepository.GetByIdsAsync(distinctGameIds); 
        
        if (games.Count() != distinctGameIds.Count)
        {
            var foundGameIds = games.Select(g => g.Id);
            var notFoundIds = distinctGameIds.Except(foundGameIds);
            throw new DomainException($"Os seguintes IDs de jogos não foram encontrados: {string.Join(", ", notFoundIds)}", HttpStatusCode.NotFound);
        }
    }

    public async Task EnsurePromotionNameIsUniqueAsync(string name)
    {
        if (await _promotionRepository.ExistsByNameAsync(name))
        {
            throw new DomainException($"Já existe uma promoção com o nome '{name}'.", HttpStatusCode.Conflict);
        }
    }
}