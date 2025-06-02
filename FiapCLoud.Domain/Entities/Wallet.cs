using FiapCLoud.Domain.Exceptions;

namespace FiapCLoud.Domain.Entities;

    public class Wallet : Entity
    {
        public Guid UserId { get; private set; }
        public DomainUser User { get; private set; } = default!;

        private decimal _balance;
        public decimal Balance => _balance;

        protected Wallet() { }

        public Wallet(Guid userId)
        {
            Validations.ValidateIfNull(userId, "O ID do usuário é obrigatório.");
            UserId = userId;
            _balance = 0m;
        }

        public void Deposit(decimal amount)
        {
            Validations.ValidateIfNegative(amount, "O valor de depósito não pode ser negativo.");
            _balance += Math.Round(amount, 2, MidpointRounding.ToEven);
        }

        public void Withdraw(decimal amount)
        {
            Validations.ValidateIfNegative(amount, "O valor de saque não pode ser negativo.");
            Validations.ValidateIfTrue(
                _balance - amount < 0m,
                "Saldo insuficiente para o saque."
            );
            _balance = Math.Round(_balance - amount, 2, MidpointRounding.ToEven);
        }
        public void Validate()
        {
            Validations.ValidateIfNull(UserId, "O ID do usuário é obrigatório.");
            Validations.ValidateIfNull(User,   "O usuário associado é obrigatório.");
            Validations.ValidateIfNegative(_balance, "O saldo não pode ser negativo.");
        }
    }
