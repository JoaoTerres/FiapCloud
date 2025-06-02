using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using FiapCLoud.Domain.Services;
using FIapCloud.App.Dtos;


namespace FIapCloud.App.Services;

public class PromotionAppService : IPromotionAppService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionDomainService _promotionDomainService;

    public PromotionAppService(
        IPromotionRepository promotionRepository,
        IGameRepository gameRepository,
        IPromotionDomainService promotionDomainService)
    {
        _promotionRepository = promotionRepository;
        _gameRepository = gameRepository;
        _promotionDomainService = promotionDomainService;
    }

    public async Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request)
    {
        await _promotionDomainService.EnsurePromotionNameIsUniqueAsync(request.Name);
        await _promotionDomainService.EnsureGamesExistAsync(request.GameIds);

        var promotionEntity = new Promotion(
            request.Name,
            request.InitialDate,
            request.FinalDate,
            request.Percentage,
            request.Enable
        );

        _promotionDomainService.ValidatePromotionRules(promotionEntity);

        foreach (var gameId in request.GameIds.Distinct())
        {
            var gamePromotion = new GamePromotion(gameId, promotionEntity.Id);
            gamePromotion.Validate();
            promotionEntity.AddGamePromotion(gamePromotion);
        }

        await _promotionRepository.AddAsync(promotionEntity);
        await _promotionRepository.CommitAsync();

        var gameIdsForResponse = promotionEntity.GamePromotions.Select(gp => gp.GameId).ToList();
        var promotionResponse = new PromotionResponse
        {
            Id = promotionEntity.Id,
            Name = promotionEntity.Name,
            InitialDate = promotionEntity.InitialDate,
            FinalDate = promotionEntity.FinalDate,
            Percentage = promotionEntity.Percentage,
            Enable = promotionEntity.Enable,
            CreatedAt = promotionEntity.CreatedAt,
            GameIds = gameIdsForResponse
        };

        return promotionResponse;
    }

    public async Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync()
    {
        var promotions = await _promotionRepository.GetAllAsync();
        if (promotions == null || !promotions.Any())
        {
            return Enumerable.Empty<PromotionResponse>();
        }

        return promotions.Select(p => new PromotionResponse
        {
            Id = p.Id,
            Name = p.Name,
            InitialDate = p.InitialDate,
            FinalDate = p.FinalDate,
            Percentage = p.Percentage,
            Enable = p.Enable,
            CreatedAt = p.CreatedAt,
            GameIds = p.GamePromotions.Select(gp => gp.GameId).ToList()
        }).ToList();
    }
}