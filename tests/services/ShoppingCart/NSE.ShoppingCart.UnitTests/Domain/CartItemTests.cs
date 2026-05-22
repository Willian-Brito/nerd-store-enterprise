using FluentAssertions;
using NSE.ShoppingCart.API.Models;
using NSE.ShoppingCart.UnitTests.Factories;

namespace NSE.ShoppingCart.UnitTests.Domain;

public class CartItemTests
{
    [Fact(DisplayName = "Deve calcular preço total do item")]
    public void Should_Calculate_Total_Item_Price()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Quantity = 2;
        item.Price = 150;

        // Act
        var result = item.CalculatePrice();

        // Assert
        result.Should().Be(300);
    }

    [Fact(DisplayName = "Deve adicionar unidades ao item")]
    public void Should_Add_Units_To_Item()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Quantity = 1;

        // Act
        item.AddUnit(3);

        // Assert
        item.Quantity.Should().Be(4);
    }

    [Fact(DisplayName = "Deve atualizar quantidade do item")]
    public void Should_Update_Item_Quantity()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Quantity = 1;

        // Act
        item.UpdateUnit(5);

        // Assert
        item.Quantity.Should().Be(5);
    }

    [Fact(DisplayName = "Deve associar item ao carrinho")]
    public void Should_Associate_Item_With_Shopping_Cart()
    {
        // Arrange
        var item = CartItemFactory.Create();
        var shoppingCartId = Guid.NewGuid();

        // Act
        item.SetShoppingCart(shoppingCartId);

        // Assert
        item.ShoppingCartId.Should().Be(shoppingCartId);
    }

    [Fact(DisplayName = "Deve retornar item válido")]
    public void Should_Return_Valid_Item()
    {
        // Arrange
        var item = CartItemFactory.Create();

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar erro quando produto for inválido")]
    public void Should_Return_Error_When_Product_Id_Is_Invalid()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.ProductId = Guid.Empty;

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeFalse();

        var validation = new CartItem.ShoppingCartItemValidation()
            .Validate(item);

        validation.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Invalid product Id");
    }

    [Fact(DisplayName = "Deve retornar erro quando nome do produto estiver vazio")]
    public void Should_Return_Error_When_Product_Name_Is_Empty()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Name = string.Empty;

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeFalse();

        var validation = new CartItem.ShoppingCartItemValidation()
            .Validate(item);

        validation.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Procut name must be set");
    }

    [Fact(DisplayName = "Deve retornar erro quando quantidade for menor que 1")]
    public void Should_Return_Error_When_Quantity_Is_Less_Than_One()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Quantity = 0;

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeFalse();

        var validation = new CartItem.ShoppingCartItemValidation()
            .Validate(item);

        validation.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                $"The minimal quantity for {item.Name} is 1");
    }

    [Fact(DisplayName = "Deve retornar erro quando quantidade exceder limite máximo")]
    public void Should_Return_Error_When_Quantity_Exceeds_Maximum_Limit()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Quantity = CustomerShoppingCart.MAX_ITEMS + 1;

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeFalse();

        var validation = new CartItem.ShoppingCartItemValidation()
            .Validate(item);

        validation.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                $"The max quantity for {item.Name} is {CustomerShoppingCart.MAX_ITEMS}");
    }

    [Fact(DisplayName = "Deve retornar erro quando preço for inválido")]
    public void Should_Return_Error_When_Price_Is_Invalid()
    {
        // Arrange
        var item = CartItemFactory.Create();
        item.Price = 0;

        // Act
        var result = item.IsValid();

        // Assert
        result.Should().BeFalse();

        var validation = new CartItem.ShoppingCartItemValidation()
            .Validate(item);

        validation.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                $"The price of {item.Name} must be greater than 0");
    }
}