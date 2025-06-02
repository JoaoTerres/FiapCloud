using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class DomainUserTests
{
    private readonly Guid _validApplicationUserId = Guid.NewGuid();
    private const string ValidName = "John Doe";


    [Fact]
    public void Constructor_ShouldCreateDomainUser_WhenValidParametersAreProvided()
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);

    
        Assert.NotNull(domainUser);
        Assert.Equal(ValidName, domainUser.Name);
        Assert.Equal(_validApplicationUserId, domainUser.ApplicationUserId);
        Assert.True(domainUser.IsActive);
        Assert.NotEqual(Guid.Empty, domainUser.Id);
        Assert.True(domainUser.CreatedAt <= DateTime.UtcNow);

        Assert.NotNull(domainUser.Library);
        Assert.Equal(domainUser.Id, domainUser.Library.UserId);

        Assert.NotNull(domainUser.Wallet);
        Assert.Equal(domainUser.Id, domainUser.Wallet.UserId);
    }


    [Theory]
    [InlineData(null, "O nome do usuário é obrigatório.")]
    [InlineData("", "O nome do usuário é obrigatório.")]
    public void Validate_ShouldThrowDomainException_WhenNameIsInvalid(string invalidName, string expectedErrorMessage)
    {
    
        var domainUser = new DomainUser("TemporaryValidName", _validApplicationUserId);
    
    
    
        var exception = Assert.Throws<DomainException>(() => domainUser.UpdateName(invalidName));


    
        Assert.Equal(expectedErrorMessage, exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenUserIsValid()
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);

    
        var exception = Record.Exception(() => domainUser.Validate());


        Assert.Null(exception);
    }



    [Fact]
    public void UpdateName_ShouldUpdateUserName_WhenNameIsValid()
    {
    
        var domainUser = new DomainUser("Old Name", _validApplicationUserId);
        var newName = "New Valid Name";

    
        domainUser.UpdateName(newName);

    
        Assert.Equal(newName, domainUser.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdateName_ShouldThrowDomainException_WhenNameIsInvalid(string invalidName)
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);

    
        var exception = Assert.Throws<DomainException>(() => domainUser.UpdateName(invalidName));

    
        Assert.Equal("O nome do usuário é obrigatório.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }


    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);
        domainUser.Deactivate();

    
        domainUser.Activate();

    
        Assert.True(domainUser.IsActive);
    }


    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);
    

    
        domainUser.Deactivate();

    
        Assert.False(domainUser.IsActive);
    }


    [Fact]
    public void Constructor_ShouldInitializeLibraryAndWallet()
    {
    
        var domainUser = new DomainUser(ValidName, _validApplicationUserId);

    
        Assert.NotNull(domainUser.Library);
        Assert.NotNull(domainUser.Wallet);
        Assert.Equal(domainUser.Id, domainUser.Library.UserId);
        Assert.Equal(domainUser.Id, domainUser.Wallet.UserId);
    }
}