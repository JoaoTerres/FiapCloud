using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class Promotion : Entity
{
    public string Name { get; private set; } = default!;
    public DateTime InitialDate { get; private set; }
    public DateTime FinalDate { get; private set; }
    public decimal Percentage { get; private set; }
    public bool Enable { get; private set; }

    private readonly List<GamePromotion> _gamePromotions = new();
    public IReadOnlyCollection<GamePromotion> GamePromotions => _gamePromotions.AsReadOnly();

    private readonly List<Sale> _sales = new();
    public IReadOnlyCollection<Sale> Sales => _sales.AsReadOnly();

    protected Promotion() { }

    public Promotion(
        string name,
        DateTime initialDate,
        DateTime finalDate,
        decimal percentage,
        bool enable)
    {
        Id = Guid.NewGuid();
        Name = name;
        InitialDate = initialDate;
        FinalDate = finalDate;
        Percentage = Math.Round(percentage, 2, MidpointRounding.ToEven);
        Enable = enable;
    }

    public void Validate()
    {
        Validations.ValidateIfNullOrEmpty(Name, "O nome da promoção é obrigatório.");
        Validations.ValidateIfTrue(InitialDate == default, "A data inicial é obrigatória.");
        Validations.ValidateIfTrue(FinalDate == default, "A data final é obrigatória.");
        Validations.ValidateIfTrue(FinalDate < InitialDate, "A data final deve ser igual ou posterior à data inicial.");
        Validations.ValidateIfNegative(Percentage, "O percentual não pode ser negativo.");
        Validations.ValidateIfTrue(Percentage > 100m, "O percentual não pode ser maior que 100.");
    }

    public void AddGamePromotion(GamePromotion gp)
    {
        Validations.ValidateIfNull(gp, "A associação GamePromotion não pode ser nula.");
        Validations.ValidateIfTrue(
            _gamePromotions.Exists(x => x.GameId == gp.GameId && x.PromotionId == gp.PromotionId),
            "Esta promoção já está aplicada a este jogo."
        );
        _gamePromotions.Add(gp);
    }

    public void RemoveGamePromotion(GamePromotion gp)
    {
        Validations.ValidateIfNull(gp, "A associação GamePromotion não pode ser nula.");
        bool removed = _gamePromotions.Remove(gp);
        Validations.ValidateIfFalse(
            removed,
            "A associação GamePromotion não foi encontrada nesta promoção."
        );
    }

    public void AddSale(Sale sale)
    {
        Validations.ValidateIfNull(sale, "A venda não pode ser nula.");
        Validations.ValidateIfTrue(
            _sales.Exists(s => s.Id == sale.Id),
            "Esta venda já está registrada nesta promoção."
        );
        _sales.Add(sale);
    }

    public void RemoveSale(Sale sale)
    {
        Validations.ValidateIfNull(sale, "A venda não pode ser nula.");
        bool removed = _sales.Remove(sale);
        Validations.ValidateIfFalse(
            removed,
            "A venda não foi encontrada nesta promoção."
        );
    }
}
