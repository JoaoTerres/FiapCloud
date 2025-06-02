using Xunit;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Exceptions;
using System;
using System.Linq;
using System.Net;

namespace FiapCloud.Tests.Domain.Entities;

public class LibraryTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validGameId = Guid.NewGuid();

    private Sale CreateValidSale()
    {
        return new Sale(_validGameId, 1, 50.00m, DateTime.UtcNow);
    }


    [Fact]
    public void Constructor_ShouldCreateLibrary_WhenValidUserIdIsProvided()
    {
        var library = new Library(_validUserId);

        Assert.NotNull(library);
        Assert.Equal(_validUserId, library.UserId);
        Assert.NotEqual(Guid.Empty, library.Id);
        Assert.True(library.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(library.Sales);
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
    public void AddSale_ShouldAddSaleToLibrary_WhenSaleIsValidAndNotDuplicate()
    {

        var library = new Library(_validUserId);
        var sale = CreateValidSale();


        library.AddSale(sale);


        Assert.Single(library.Sales);
        Assert.Contains(sale, library.Sales);
    }

    [Fact]
    public void AddSale_ShouldThrowDomainException_WhenSaleIsNull()
    {

        var library = new Library(_validUserId);


        var exception = Assert.Throws<DomainException>(() => library.AddSale(null!));


        Assert.Equal("A venda não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void AddSale_ShouldThrowDomainException_WhenSaleAlreadyExists()
    {
        
        var library = new Library(_validUserId);
        var sale = CreateValidSale();
        library.AddSale(sale); 
        
        var exception = Assert.Throws<DomainException>(() => library.AddSale(sale)); 

        Assert.Equal("Esta venda já está registrada nesta biblioteca.", exception.Message);
        
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }


    [Fact]
    public void RemoveSale_ShouldRemoveSaleFromLibrary_WhenSaleExists()
    {

        var library = new Library(_validUserId);
        var sale = CreateValidSale();
        library.AddSale(sale);

        library.RemoveSale(sale);

        Assert.Empty(library.Sales);
    }

    [Fact]
    public void RemoveSale_ShouldThrowDomainException_WhenSaleIsNull()
    {

        var library = new Library(_validUserId);

        var exception = Assert.Throws<DomainException>(() => library.RemoveSale(null!));

        Assert.Equal("A venda não pode ser nula.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void RemoveSale_ShouldThrowDomainException_WhenSaleDoesNotExist()
    {

        var library = new Library(_validUserId);
        var saleToRemove = CreateValidSale();

        var exception = Assert.Throws<DomainException>(() => library.RemoveSale(saleToRemove));

        Assert.Equal("A venda não foi encontrada nesta biblioteca.", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }


    [Fact]
    public void Validate_ShouldNotThrowException_WhenLibraryIsValid()
    {
        var library = new Library(_validUserId);

        var exception = Record.Exception(() => library.Validate());

        Assert.Null(exception);
    }
}