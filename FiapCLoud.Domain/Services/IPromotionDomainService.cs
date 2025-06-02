using FiapCLoud.Domain.Entities;

namespace FiapCLoud.Domain.Services;

public interface IPromotionDomainService
{
    void ValidatePromotionRules(Promotion promotion);
    Task EnsureGamesExistAsync(IEnumerable<Guid> gameIds);
    Task EnsurePromotionNameIsUniqueAsync(string name);
}