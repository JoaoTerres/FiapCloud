using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class SaleTests
{
    private readonly Guid _validGameId = Guid.NewGuid();
    private readonly int _validQuantity = 2;
    private readonly decimal _validUnitPrice = 29.99m;
    private readonly DateTime _validSaleDate = DateTime.UtcNow;

    
    [Fact]
    public void Constructor_ShouldCreateSale_WhenValidParametersAreProvided()
    {
        
        var sale = new Sale(_validGameId, _validQuantity, _validUnitPrice, _validSaleDate);

        Assert.NotNull(sale);
        Assert.Equal(_validGameId, sale.GameId);
        Assert.Equal(_validQuantity, sale.Quantity);
        Assert.Equal(_validUnitPrice, sale.UnitPrice);
        Assert.Equal(_validSaleDate, sale.SaleDate);
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.True(sale.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(_validQuantity * _validUnitPrice, sale.TotalPrice); 
    }

    
    [Fact]
    public void Validate_ShouldNotThrowException_WhenSaleIsValid()
    {
        
        var sale = new Sale(_validGameId, _validQuantity, _validUnitPrice, _validSaleDate);

        var exception = Record.Exception(() => sale.Validate());

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_ShouldThrowDomainException_WhenGameIdIsEmpty()
    {
        
        var invalidGameId = Guid.Empty;
 
        
        Action act = () => FiapCLoud.Domain.Exceptions.Validations.ValidateIfTrue(invalidGameId == Guid.Empty, "O ID do jogo é obrigatório.");
        var exception = Assert.Throws<DomainException>(act);

        Assert.Equal("O ID do jogo é obrigatório.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
  
    }


    [Theory]
    [InlineData(0, "A quantidade vendida deve ser maior que zero.")]
    [InlineData(-1, "A quantidade vendida deve ser maior que zero.")]
    public void Validate_ShouldThrowDomainException_WhenQuantityIsInvalid(int invalidQuantity, string expectedErrorMessage)
    {
        
        var exception = Assert.Throws<DomainException>(() =>
        {
            var sale = new Sale(_validGameId, invalidQuantity, _validUnitPrice, _validSaleDate);
            sale.Validate();
        });

        
        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void Validate_ShouldThrowDomainException_WhenUnitPriceIsNegative()
    {
        
        var invalidUnitPrice = -5.00m;

        
        var exception = Assert.Throws<DomainException>(() =>
        {
            var sale = new Sale(_validGameId, _validQuantity, invalidUnitPrice, _validSaleDate);
            sale.Validate();
        });

        
        Assert.Equal("O preço unitário não pode ser negativo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    
    [Theory]
    [InlineData(1, 10.00, 10.00)]
    [InlineData(3, 15.50, 46.50)]
    [InlineData(5, 0.00, 0.00)] 
    public void TotalPrice_ShouldBeCalculatedCorrectly(int quantity, decimal unitPrice, decimal expectedTotalPrice)
    {
        
        var sale = new Sale(_validGameId, quantity, unitPrice, _validSaleDate);

        
        var actualTotalPrice = sale.TotalPrice;

        
        Assert.Equal(expectedTotalPrice, actualTotalPrice);
    }
}