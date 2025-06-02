using FIapCloud.App.Dtos;

namespace FIapCloud.App.Services;

public interface IPromotionAppService
{
    Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request);
    Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync();
}