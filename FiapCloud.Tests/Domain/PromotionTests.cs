using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Linq;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class PromotionTests
{
    private readonly string _validName = "Summer Sale";
    private readonly DateTime _validInitialDate = DateTime.UtcNow.AddDays(1);
    private readonly DateTime _validFinalDate = DateTime.UtcNow.AddDays(10);
    private readonly decimal _validPercentage = 15.50m;
    private readonly bool _validEnableStatus = true;

    private GamePromotion CreateValidGamePromotion(Guid promotionId)
    {
        return new GamePromotion(Guid.NewGuid(), promotionId);
    }

    private Sale CreateValidSale()
    {
        return new Sale(Guid.NewGuid(), 1, 100m, DateTime.UtcNow);
    }

    
    [Fact]
    public void Constructor_ShouldCreatePromotion_WhenValidParametersAreProvided()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        promotion.Validate(); 

        
        Assert.NotNull(promotion);
        Assert.Equal(_validName, promotion.Name);
        Assert.Equal(_validInitialDate, promotion.InitialDate);
        Assert.Equal(_validFinalDate, promotion.FinalDate);
        Assert.Equal(Math.Round(_validPercentage, 2, MidpointRounding.ToEven), promotion.Percentage);
        Assert.Equal(_validEnableStatus, promotion.Enable);
        Assert.NotEqual(Guid.Empty, promotion.Id);
        Assert.True(promotion.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(promotion.GamePromotions);
        Assert.Empty(promotion.Sales);
    }

    [Theory]
    [InlineData(null, "2025-07-01", "2025-07-10", 10, true, "O nome da promoção é obrigatório.")]
    [InlineData("", "2025-07-01", "2025-07-10", 10, true, "O nome da promoção é obrigatório.")]
    [InlineData("Valid Name", "0001-01-01", "2025-07-10", 10, true, "A data inicial é obrigatória.")] 
    [InlineData("Valid Name", "2025-07-01", "0001-01-01", 10, true, "A data final é obrigatória.")]   
    [InlineData("Valid Name", "2025-07-10", "2025-07-01", 10, true, "A data final deve ser igual ou posterior à data inicial.")]
    [InlineData("Valid Name", "2025-07-01", "2025-07-10", -5, true, "O percentual não pode ser negativo.")]
    [InlineData("Valid Name", "2025-07-01", "2025-07-10", 101, true, "O percentual não pode ser maior que 100.")]
    public void Constructor_And_Validate_ShouldThrowDomainException_WhenInvalidParametersAreProvided(
        string name, string initialDateStr, string finalDateStr, decimal percentage, bool enable, string expectedErrorMessage)
    {
        
        DateTime initialDate = initialDateStr == "0001-01-01" ? default : DateTime.Parse(initialDateStr);
        DateTime finalDate = finalDateStr == "0001-01-01" ? default : DateTime.Parse(finalDateStr);

        
        var exception = Assert.Throws<DomainException>(() =>
        {
            var promotion = new Promotion(name, initialDate, finalDate, percentage, enable);
            promotion.Validate();
        });

        
        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void Constructor_ShouldRoundPercentageToTwoDecimalPlaces()
    {
        
        var percentage = 15.125m;
        var expectedRoundedPercentage = 15.12m; 

        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, percentage, _validEnableStatus);

        
        Assert.Equal(expectedRoundedPercentage, promotion.Percentage);
    }


    
    [Fact]
    public void AddGamePromotion_ShouldAddGamePromotion_WhenValidAndNotDuplicate()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var gamePromotion = CreateValidGamePromotion(promotion.Id);

        
        promotion.AddGamePromotion(gamePromotion);

        
        Assert.Single(promotion.GamePromotions);
        Assert.Contains(gamePromotion, promotion.GamePromotions);
    }

    [Fact]
    public void AddGamePromotion_ShouldThrowDomainException_WhenGamePromotionIsNull()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);

        
        var exception = Assert.Throws<DomainException>(() => promotion.AddGamePromotion(null!));

        
        Assert.Equal("A associação GamePromotion não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void AddGamePromotion_ShouldThrowDomainException_WhenGamePromotionAlreadyExists()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var gamePromotion = CreateValidGamePromotion(promotion.Id);
        promotion.AddGamePromotion(gamePromotion); 

        
        var exception = Assert.Throws<DomainException>(() => promotion.AddGamePromotion(gamePromotion)); 

        
        Assert.Equal("Esta promoção já está aplicada a este jogo.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    
    [Fact]
    public void RemoveGamePromotion_ShouldRemoveGamePromotion_WhenExists()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var gamePromotion = CreateValidGamePromotion(promotion.Id);
        promotion.AddGamePromotion(gamePromotion);

        
        promotion.RemoveGamePromotion(gamePromotion);

        
        Assert.Empty(promotion.GamePromotions);
    }

    [Fact]
    public void RemoveGamePromotion_ShouldThrowDomainException_WhenGamePromotionIsNull()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);

        
        var exception = Assert.Throws<DomainException>(() => promotion.RemoveGamePromotion(null!));

        
        Assert.Equal("A associação GamePromotion não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void RemoveGamePromotion_ShouldThrowDomainException_WhenGamePromotionDoesNotExist()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var gamePromotionToRemove = CreateValidGamePromotion(promotion.Id);

        
        var exception = Assert.Throws<DomainException>(() => promotion.RemoveGamePromotion(gamePromotionToRemove));

        Assert.Equal("A associação GamePromotion não foi encontrada nesta promoção.", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }

    
    [Fact]
    public void AddSale_ShouldAddSaleToList_WhenSaleIsValidAndNotDuplicate()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var sale = CreateValidSale();

        promotion.AddSale(sale);

        Assert.Single(promotion.Sales);
        Assert.Contains(sale, promotion.Sales);
    }

    [Fact]
    public void AddSale_ShouldThrowDomainException_WhenSaleIsNull()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);

        var exception = Assert.Throws<DomainException>(() => promotion.AddSale(null!));

        Assert.Equal("A venda não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void AddSale_ShouldThrowDomainException_WhenSaleAlreadyExists()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var sale = CreateValidSale();
        promotion.AddSale(sale);

        var exception = Assert.Throws<DomainException>(() => promotion.AddSale(sale));

        Assert.Equal("Esta venda já está registrada nesta promoção.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    
    [Fact]
    public void RemoveSale_ShouldRemoveSaleFromList_WhenSaleExists()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var sale = CreateValidSale();
        promotion.AddSale(sale);

        promotion.RemoveSale(sale);

        Assert.Empty(promotion.Sales);
    }

    [Fact]
    public void RemoveSale_ShouldThrowDomainException_WhenSaleIsNull()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);

        var exception = Assert.Throws<DomainException>(() => promotion.RemoveSale(null!));

        Assert.Equal("A venda não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void RemoveSale_ShouldThrowDomainException_WhenSaleDoesNotExist()
    {
        
        var promotion = new Promotion(_validName, _validInitialDate, _validFinalDate, _validPercentage, _validEnableStatus);
        var saleToRemove = CreateValidSale();

        
        var exception = Assert.Throws<DomainException>(() => promotion.RemoveSale(saleToRemove));

        Assert.Equal("A venda não foi encontrada nesta promoção.", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }
}