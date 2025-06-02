using System.Net;
using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class Game : Entity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    private readonly List<GamePromotion> _gamePromotions = new();
    public IReadOnlyCollection<GamePromotion> GamePromotions => _gamePromotions.AsReadOnly();

    private readonly List<Sale> _sales = new();
    public IReadOnlyCollection<Sale> Sales => _sales.AsReadOnly();

    protected Game() { }

    public Game(string name, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        Validations.ValidateIfNullOrEmpty(name, "O nome do jogo é obrigatório.");
        Validations.ValidateIfNullOrEmpty(description, "A descrição do jogo é obrigatória.");
        Validations.ValidateIfNegative(price, "O preço do jogo não pode ser negativo.");
        Validations.ValidateIfNull(price, "O preço do jogo é obrigatório.");

        Name = name;
        Description = description;
        Price = price;
    }

    public void AddPromotion(GamePromotion promotion)
    {
        Validations.ValidateIfNull(promotion, "A promoção não pode ser nula.");
        if (_gamePromotions.Exists(p => p.PromotionId == promotion.PromotionId))
            throw new DomainException("Esta promoção já está aplicada ao jogo.", HttpStatusCode.BadRequest);

        _gamePromotions.Add(promotion);
    }

    public void RemovePromotion(GamePromotion promotion)
    {
        Validations.ValidateIfNull(promotion, "A promoção não pode ser nula.");
        if (!_gamePromotions.Remove(promotion))
            throw new DomainException("A promoção não foi encontrada no jogo.", HttpStatusCode.NotFound);
    }

    public void RecordSale(int quantity, DateTime saleDate)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade vendida deve ser maior que zero.", HttpStatusCode.BadRequest);

        var sale = new Sale(Id, quantity, Price, saleDate);
        _sales.Add(sale);
    }

    public void Validate()
    {
        Validations.ValidateIfNullOrEmpty(Name, "O nome do jogo é obrigatório.");
        Validations.ValidateIfNullOrEmpty(Description, "A descrição do jogo é obrigatória.");
        Validations.ValidateIfNegative(Price, "O preço do jogo não pode ser negativo.");
        Validations.ValidateIfNull(Price, "O preço do jogo é obrigatório.");
    }
}