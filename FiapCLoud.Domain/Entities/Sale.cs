using System.Net;
using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class Sale : Entity
{
    public Guid GameId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public DateTime SaleDate { get; private set; }

    protected Sale() { }

    public Sale(Guid gameId, int quantity, decimal unitPrice, DateTime saleDate)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        SaleDate = saleDate;
    }

    public void Validate()
    {
        Validations.ValidateIfNull(GameId, "O ID do jogo é obrigatório.");
        Validations.ValidateIfLessOrEqualZero(Quantity, "A quantidade vendida deve ser maior que zero.");
        Validations.ValidateIfNegative(UnitPrice, "O preço unitário não pode ser negativo.");
    }
}
