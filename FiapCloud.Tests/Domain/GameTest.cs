using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Linq;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class GameTests
{


    [Fact]
    public void Constructor_ShouldCreateGame_WhenValidParametersAreProvided()
    {
    
        var name = "Super Test Game";
        var description = "An exciting game of tests.";
        var price = 49.99m;

    
        var game = new Game(name, description, price);
        game.Validate();

    
        Assert.NotNull(game);
        Assert.Equal(name, game.Name);
        Assert.Equal(description, game.Description);
        Assert.Equal(price, game.Price);
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.True(game.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(game.GamePromotions);
        Assert.Empty(game.Sales);
    }

    [Theory]
    [InlineData(null, "Valid Description", 10.00, "O nome do jogo é obrigatório.")]
    [InlineData("", "Valid Description", 10.00, "O nome do jogo é obrigatório.")]
    [InlineData("Valid Name", null, 10.00, "A descrição do jogo é obrigatória.")]
    [InlineData("Valid Name", "", 10.00, "A descrição do jogo é obrigatória.")]
    [InlineData("Valid Name", "Valid Description", -1.00, "O preço do jogo não pode ser negativo.")]
    public void Constructor_And_Validate_ShouldThrowDomainException_WhenInvalidParametersAreProvided(string name, string description, decimal price, string expectedErrorMessage)
    {
    
        var exception = Assert.Throws<DomainException>(() =>
        {
            var game = new Game(name, description, price);
            game.Validate();
        });

    
        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }



    [Fact]
    public void UpdateDetails_ShouldUpdateGameProperties_WhenValidParametersAreProvided()
    {
    
        var game = new Game("Old Name", "Old Description", 10.0m);
        var newName = "New Name";
        var newDescription = "New Description";
        var newPrice = 20.0m;

    
        game.UpdateDetails(newName, newDescription, newPrice);

    
        Assert.Equal(newName, game.Name);
        Assert.Equal(newDescription, game.Description);
        Assert.Equal(newPrice, game.Price);
    }

    [Theory]
    [InlineData(null, "New Description", 20.0, "O nome do jogo é obrigatório.")]
    [InlineData("", "New Description", 20.0, "O nome do jogo é obrigatório.")]
    [InlineData("New Name", null, 20.0, "A descrição do jogo é obrigatória.")]
    [InlineData("New Name", "", 20.0, "A descrição do jogo é obrigatória.")]
    [InlineData("New Name", "New Description", -5.0, "O preço do jogo não pode ser negativo.")]
    public void UpdateDetails_ShouldThrowDomainException_WhenInvalidParametersAreProvided(string name, string description, decimal price, string expectedErrorMessage)
    {
    
        var game = new Game("Original Name", "Original Description", 15.0m);

    
        var exception = Assert.Throws<DomainException>(() => game.UpdateDetails(name, description, price));

    
        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }



    [Fact]
    public void AddPromotion_ShouldAddPromotionToGame_WhenPromotionIsValidAndNotDuplicate()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var promotionId = Guid.NewGuid();
        var gamePromotion = new GamePromotion(game.Id, promotionId);

    
        game.AddPromotion(gamePromotion);

    
        Assert.Single(game.GamePromotions);
        Assert.Contains(gamePromotion, game.GamePromotions);
    }

    [Fact]
    public void AddPromotion_ShouldThrowDomainException_WhenPromotionIsNull()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);

    
        var exception = Assert.Throws<DomainException>(() => game.AddPromotion(null!));

    
        Assert.Equal("A promoção não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void AddPromotion_ShouldThrowDomainException_WhenPromotionAlreadyExists()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var promotionId = Guid.NewGuid();
        var gamePromotion = new GamePromotion(game.Id, promotionId);
        game.AddPromotion(gamePromotion);

    
        var exception = Assert.Throws<DomainException>(() => game.AddPromotion(gamePromotion));

    
        Assert.Equal("Esta promoção já está aplicada ao jogo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }



    [Fact]
    public void RemovePromotion_ShouldRemovePromotionFromGame_WhenPromotionExists()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var promotionId = Guid.NewGuid();
        var gamePromotion = new GamePromotion(game.Id, promotionId);
        game.AddPromotion(gamePromotion);

    
        game.RemovePromotion(gamePromotion);

    
        Assert.Empty(game.GamePromotions);
    }

    [Fact]
    public void RemovePromotion_ShouldThrowDomainException_WhenPromotionIsNull()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);

    
        var exception = Assert.Throws<DomainException>(() => game.RemovePromotion(null!));

    
        Assert.Equal("A promoção não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void RemovePromotion_ShouldThrowDomainException_WhenPromotionDoesNotExist()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var promotionId = Guid.NewGuid();
        var gamePromotionToRemove = new GamePromotion(game.Id, promotionId);

    
        var exception = Assert.Throws<DomainException>(() => game.RemovePromotion(gamePromotionToRemove));

    
        Assert.Equal("A promoção não foi encontrada no jogo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }



    [Fact]
    public void RecordSale_ShouldAddSaleToGame_WhenQuantityIsValid()
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var quantity = 2;
        var saleDate = DateTime.UtcNow;

    
        game.RecordSale(quantity, saleDate);

    
        Assert.Single(game.Sales);
        var recordedSale = game.Sales.First();
        Assert.Equal(game.Id, recordedSale.GameId);
        Assert.Equal(quantity, recordedSale.Quantity);
        Assert.Equal(game.Price, recordedSale.UnitPrice);
        Assert.Equal(saleDate, recordedSale.SaleDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RecordSale_ShouldThrowDomainException_WhenQuantityIsInvalid(int quantity)
    {
    
        var game = new Game("Test Game", "Description", 29.99m);
        var saleDate = DateTime.UtcNow;

    
        var exception = Assert.Throws<DomainException>(() => game.RecordSale(quantity, saleDate));

    
        Assert.Equal("A quantidade vendida deve ser maior que zero.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }
}