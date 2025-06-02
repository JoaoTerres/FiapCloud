using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class GamePromotion : Entity
{
    public Guid GameId { get; private set; }
    public Game Game { get; private set; } = default!;
    public Guid PromotionId { get; private set; }

    protected GamePromotion() { }

    public GamePromotion(Guid gameId, Guid promotionId)
    {
        GameId = gameId;
        PromotionId = promotionId;
    }

    public void Validate()
    {
        Validations.ValidateIfNull(PromotionId, "O ID da promoção é obrigatório.");
    }
}
