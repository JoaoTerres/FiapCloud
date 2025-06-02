using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

public class DomainUser : Entity
{
    public string Name { get; private set; } = default!;
    public Guid ApplicationUserId { get; private set; } = default!;
    public bool IsActive { get; private set; }
    private Library _library = default!;
    public Library Library => _library;
    private Wallet _wallet = default!;
    public Wallet Wallet => _wallet;

    protected DomainUser() { }

    public DomainUser(string name, Guid applicationUserId)
    {
        Id = Guid.NewGuid();
        Name = name;
        ApplicationUserId = applicationUserId;
        IsActive = true;

        _library = new Library(Id);
        _wallet = new Wallet(Id);
    }

    public void UpdateName(string name)
    {
        Validations.ValidateIfNullOrEmpty(name, "O nome do usuário é obrigatório.");
        Name = name;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Validate()
    {
        Validations.ValidateIfNullOrEmpty(Name, "O nome do usuário é obrigatório.");
        Validations.ValidateIfNull(_library, "A biblioteca do usuário é obrigatória.");
        Validations.ValidateIfNull(_wallet, "A carteira do usuário é obrigatória.");
    }
}
