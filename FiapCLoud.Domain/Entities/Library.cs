using System.Net;
using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class Library : Entity
{
    public Guid UserId { get; private set; }
    public DomainUser User { get; private set; } = default!;
    private readonly List<Sale> _sales = new();
    public IReadOnlyCollection<Sale> Sales => _sales.AsReadOnly();
    protected Library() { }

    public Library(Guid userId)
    {
        UserId = userId;
        Validate();
    }

    public void AddSale(Sale sale)
    {
        Validations.ValidateIfNull(sale, "A venda não pode ser nula.");
        Validations.ValidateIfTrue(
            _sales.Exists(s => s.Id == sale.Id),
            "Esta venda já está registrada nesta biblioteca."
        );

        _sales.Add(sale);
    }

    public void RemoveSale(Sale sale)
    {
        Validations.ValidateIfNull(sale, "A venda não pode ser nula.");
        bool removed = _sales.Remove(sale);
        Validations.ValidateIfFalse(
            removed,
            "A venda não foi encontrada nesta biblioteca."
        );
    }

    public void Validate()
    {
        Validations.ValidateIfNull(UserId, "O ID do usuário é obrigatório.");
    }
}
