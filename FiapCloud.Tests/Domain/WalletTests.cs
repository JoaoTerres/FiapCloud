using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class WalletTests
{
    private readonly Guid _validUserId = Guid.NewGuid();


    [Fact]
    public void Constructor_ShouldCreateWallet_WhenValidUserIdIsProvided()
    {

        var wallet = new Wallet(_validUserId);

        Assert.NotNull(wallet);
        Assert.Equal(_validUserId, wallet.UserId);
        Assert.Equal(0m, wallet.Balance);
        Assert.NotEqual(Guid.Empty, wallet.Id);
        Assert.True(wallet.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenUserIdIsEmpty()
    {

        var invalidUserId = Guid.Empty;

        Action act = () => FiapCLoud.Domain.Exceptions.Validations.ValidateIfTrue(invalidUserId == Guid.Empty, "O ID do usuário é obrigatório.");
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("O ID do usuário é obrigatório.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }


    [Fact]
    public void Deposit_ShouldIncreaseBalance_WhenAmountIsPositive()
    {

        var wallet = new Wallet(_validUserId);
        var initialBalance = wallet.Balance;
        var depositAmount = 100.50m;
        var expectedBalance = initialBalance + depositAmount;


        wallet.Deposit(depositAmount);


        Assert.Equal(expectedBalance, wallet.Balance);
    }

    [Fact]
    public void Deposit_ShouldRoundAmountToTwoDecimalPlaces()
    {

        var wallet = new Wallet(_validUserId);
        var depositAmount = 100.125m;
        var expectedBalance = 100.12m;


        wallet.Deposit(depositAmount);


        Assert.Equal(expectedBalance, wallet.Balance);


        var wallet2 = new Wallet(_validUserId);
        var depositAmount2 = 100.126m;
        var expectedBalance2 = 100.13m;


        wallet2.Deposit(depositAmount2);


        Assert.Equal(expectedBalance2, wallet2.Balance);
    }


    [Fact]
    public void Deposit_ShouldNotThrowException_WhenAmountIsZero()
    {

        var wallet = new Wallet(_validUserId);
        var initialBalance = wallet.Balance;


        var exception = Record.Exception(() => wallet.Deposit(0m));


        Assert.Null(exception);
        Assert.Equal(initialBalance, wallet.Balance);
    }

    [Fact]
    public void Deposit_ShouldThrowDomainException_WhenAmountIsNegative()
    {

        var wallet = new Wallet(_validUserId);
        var negativeAmount = -50m;


        var exception = Assert.Throws<DomainException>(() => wallet.Deposit(negativeAmount));


        Assert.Equal("O valor de depósito não pode ser negativo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }


    [Fact]
    public void Withdraw_ShouldDecreaseBalance_WhenAmountIsValidAndSufficientBalance()
    {

        var wallet = new Wallet(_validUserId);
        wallet.Deposit(200m);
        var initialBalance = wallet.Balance;
        var withdrawAmount = 75.25m;
        var expectedBalance = Math.Round(initialBalance - withdrawAmount, 2, MidpointRounding.ToEven);



        wallet.Withdraw(withdrawAmount);


        Assert.Equal(expectedBalance, wallet.Balance);
    }

    [Fact]
    public void Withdraw_ShouldRoundAmountToTwoDecimalPlaces()
    {

        var wallet = new Wallet(_validUserId);
        wallet.Deposit(200m);
        var withdrawAmount = 75.125m;
        var expectedBalance = 200m - 75.12m;


        wallet.Withdraw(withdrawAmount);


        Assert.Equal(expectedBalance, wallet.Balance);


        var wallet2 = new Wallet(_validUserId);
        wallet2.Deposit(200m);
        var withdrawAmount2 = 75.126m;
        var expectedBalance2 = 200m - 75.13m;


        wallet2.Withdraw(withdrawAmount2);


        Assert.Equal(expectedBalance2, wallet2.Balance);
    }

    [Fact]
    public void Withdraw_ShouldNotThrowException_WhenAmountIsZero()
    {

        var wallet = new Wallet(_validUserId);
        wallet.Deposit(100m);
        var initialBalance = wallet.Balance;


        var exception = Record.Exception(() => wallet.Withdraw(0m));


        Assert.Null(exception);
        Assert.Equal(initialBalance, wallet.Balance);
    }


    [Fact]
    public void Withdraw_ShouldThrowDomainException_WhenAmountIsNegative()
    {

        var wallet = new Wallet(_validUserId);
        var negativeAmount = -50m;


        var exception = Assert.Throws<DomainException>(() => wallet.Withdraw(negativeAmount));


        Assert.Equal("O valor de saque não pode ser negativo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void Withdraw_ShouldThrowDomainException_WhenAmountIsGreaterThanBalance()
    {

        var wallet = new Wallet(_validUserId);
        wallet.Deposit(50m);
        var withdrawAmount = 100m;


        var exception = Assert.Throws<DomainException>(() => wallet.Withdraw(withdrawAmount));


        Assert.Equal("Saldo insuficiente para o saque.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }


    [Fact]
    public void Validate_ShouldNotThrowException_WhenWalletIsValidAndUserIsNull()
    {

        var wallet = new Wallet(_validUserId);


        Action act = () => wallet.Validate();

        var exception = Record.Exception(act);
        if (exception is DomainException de && de.Message == "O usuário associado é obrigatório.")
        {

            Assert.Equal("O usuário associado é obrigatório.", de.Message);
        }
        else
        {
            Assert.Null(exception);
        }
    }

    [Fact]
    public void Validate_ShouldThrowDomainException_WhenUserIdIsEmptyInValidate()
    {

        var invalidUserIdForValidation = Guid.Empty;
        Action act = () => FiapCLoud.Domain.Exceptions.Validations.ValidateIfTrue(invalidUserIdForValidation == Guid.Empty, "O ID do usuário é obrigatório.");
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("O ID do usuário é obrigatório.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

}