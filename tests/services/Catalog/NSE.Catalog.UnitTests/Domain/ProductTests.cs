using FluentAssertions;
using NSE.Catalog.API.Models.Entities;

namespace NSE.Catalog.UnitTests.Domain;

public class ProductTests
{
    [Fact(DisplayName = "Deve reduzir o estoque quando houver quantidade disponível")]
    public void ShouldDecreaseStockWhenHasAvailableQuantity()
    {
        // Arrange
        var product = new Product
        {
            Active = true,
            Stock = 10
        };

        // Act
        product.TakeFromInventory(3);

        // Assert
        product.Stock.Should().Be(7);
    }

    [Fact(DisplayName = "Não se deve diminuir o estoque quando a quantidade for maior que o estoque.")]
    public void Should_Not_Decrease_Stock_When_Quantity_Is_Greater_Than_Stock()
    {
        // Arrange
        var product = new Product
        {
            Active = true,
            Stock = 5
        };

        // Act
        product.TakeFromInventory(10);

        // Assert
        product.Stock.Should().Be(5);
    }

    [Theory(DisplayName="Deveria validar a disponibilidade do produto")]
    [InlineData(true, 10, 5, true)]
    [InlineData(true, 1, 5, false)]
    [InlineData(false, 10, 5, false)]
    public void Should_Validate_Product_Availability(
        bool active,
        int stock,
        int quantity,
        bool expectedResult
    )
    {
        // Arrange
        var product = new Product
        {
            Active = active,
            Stock = stock
        };

        // Act
        var result = product.IsAvailable(quantity);

        // Assert
        result.Should().Be(expectedResult);
    }
}