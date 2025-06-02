using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class GamePromotionTests
{
    private readonly Guid _validGameId = Guid.NewGuid();
    private readonly Guid _validPromotionId = Guid.NewGuid();

    
    [Fact]
    public void Constructor_ShouldCreateGamePromotion_WhenValidParametersAreProvided()
    {
        
        var gamePromotion = new GamePromotion(_validGameId, _validPromotionId);

        
        Assert.NotNull(gamePromotion);
        Assert.Equal(_validGameId, gamePromotion.GameId);
        Assert.Equal(_validPromotionId, gamePromotion.PromotionId);
        Assert.NotEqual(Guid.Empty, gamePromotion.Id);
        Assert.True(gamePromotion.CreatedAt <= DateTime.UtcNow);
    }

    
    [Fact]
    public void Validate_ShouldNotThrowException_WhenPromotionIdIsValid()
    {
        
        var gamePromotion = new GamePromotion(_validGameId, _validPromotionId);

        
        var exception = Record.Exception(() => gamePromotion.Validate());

        
        Assert.Null(exception);
    }
    
    [Fact]
    public void Validate_ShouldThrowDomainException_WhenPromotionIdIsEmpty()
    {
        
        var gameId = Guid.NewGuid();
        var invalidPromotionId = Guid.Empty; 
        var gamePromotion = new GamePromotion(gameId, invalidPromotionId);
        Action act = () => FiapCLoud.Domain.Exceptions.Validations.ValidateIfTrue(invalidPromotionId == Guid.Empty, "O ID da promoção é obrigatório.");
        var exception = Assert.Throws<DomainException>(act);
        Assert.Equal("O ID da promoção é obrigatório.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
        
    }


    [Fact]
    public void Constructor_ShouldAssignGameIdAndPromotionIdCorrectly()
    {
        Guid gameId = Guid.NewGuid();
        Guid promotionId = Guid.NewGuid();   
        var gamePromotion = new GamePromotion(gameId, promotionId);

        Assert.Equal(gameId, gamePromotion.GameId);
        Assert.Equal(promotionId, gamePromotion.PromotionId);
    }
}