using FluentAssertions;
using NSE.ShoppingCart.API.Models;
using NSE.ShoppingCart.UnitTests.Factories;

namespace NSE.ShoppingCart.UnitTests.Domain;

public class CustomerShoppingCartTests
{
    [Fact(DisplayName = "Deve adicionar item no carrinho")]
    public void Should_Add_Item_To_Shopping_Cart()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Mouse Gamer",
            2,
            100
        );

        // Act
        cart.AddItem(item);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.Total.Should().Be(200);
    }

    [Fact(DisplayName = "Deve somar quantidade ao adicionar item já existente")]
    public void Should_Sum_Item_Quantity_When_Item_Already_Exists()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var productId = Guid.NewGuid();

        var item1 = CartItemFactory.Create(productId, "Mouse", 1, 100);
        var item2 = CartItemFactory.Create(productId, "Mouse", 2, 100);

        // Act
        cart.AddItem(item1);
        cart.AddItem(item2);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.Items.First().Quantity.Should().Be(3);
        cart.Total.Should().Be(300);
    }

    [Fact(DisplayName = "Deve atualizar item do carrinho")]
    public void Should_Update_Shopping_Cart_Item()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();
        var productId = Guid.NewGuid();
        var item = CartItemFactory.Create(productId, "Keyboard", 1, 200);

        cart.AddItem(item);
        item.UpdateUnit(3);

        // Act
        cart.UpdateItem(item);

        // Assert
        cart.Items.First().Quantity.Should().Be(3);
        cart.Total.Should().Be(600);
    }

    [Fact(DisplayName = "Deve atualizar quantidade do item")]
    public void Should_Update_Item_Quantity()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Headset",
            1,
            150
        );

        cart.AddItem(item);

        // Act
        cart.UpdateUnit(item, 4);

        // Assert
        cart.Items.First().Quantity.Should().Be(4);
        cart.Total.Should().Be(600);
    }

    [Fact(DisplayName = "Deve remover item do carrinho")]
    public void Should_Remove_Item_From_Shopping_Cart()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Monitor",
            1,
            1000
        );

        cart.AddItem(item);

        // Act
        cart.RemoveItem(item);

        // Assert
        cart.Items.Should().BeEmpty();
        cart.Total.Should().Be(0);
    }

    [Fact(DisplayName = "Deve verificar se item existe no carrinho")]
    public void Should_Check_If_Item_Exists_In_Shopping_Cart()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Notebook",
            1,
            5000
        );

        cart.AddItem(item);

        // Act
        var result = cart.HasItem(item);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve obter item pelo id do produto")]
    public void Should_Get_Item_By_Product_Id()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var productId = Guid.NewGuid();

        var item = CartItemFactory.Create(
            productId,
            "Webcam",
            1,
            300
        );

        cart.AddItem(item);

        // Act
        var result = cart.GetProductById(productId);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(productId);
    }

    [Fact(DisplayName = "Deve aplicar voucher percentual")]
    public void Should_Apply_Percentage_Voucher()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Cadeira Gamer",
            1,
            1000
        );

        cart.AddItem(item);
        
        var voucher = new Voucher
        {
            Code = "PROMO10",
            Percentage = 10,
            Discount = null,
            DiscountType = DiscountType.Percentage
        };

        // Act
        cart.ApplyVoucher(voucher);

        // Assert
        cart.HasVoucher.Should().BeTrue();
        cart.Discount.Should().Be(100);
        cart.Total.Should().Be(900);
    }

    [Fact(DisplayName = "Deve aplicar voucher de valor fixo")]
    public void Should_Apply_Fixed_Amount_Voucher()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Mesa",
            1,
            500
        );

        cart.AddItem(item);

        var voucher = new Voucher
        {
            Code = "PROMO50",
            Percentage = null,
            Discount = 50,
            DiscountType = DiscountType.Value
        };

        // Act
        cart.ApplyVoucher(voucher);

        // Assert
        cart.Discount.Should().Be(50);
        cart.Total.Should().Be(450);
    }

    [Fact(DisplayName = "Deve zerar total quando desconto for maior que valor total")]
    public void Should_Set_Total_To_Zero_When_Discount_Is_Greater_Than_Total()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Mouse Pad",
            1,
            20
        );

        cart.AddItem(item);
        
        var voucher = new Voucher
        {
            Code = "PROMO100",
            Percentage = null,
            Discount = 100,
            DiscountType = DiscountType.Value
        };

        // Act
        cart.ApplyVoucher(voucher);

        // Assert
        cart.Total.Should().Be(0);
    }

    [Fact(DisplayName = "Deve retornar carrinho válido")]
    public void Should_Return_Valid_Shopping_Cart()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "SSD",
            1,
            500
        );

        cart.AddItem(item);

        // Act
        var result = cart.IsValid();

        // Assert
        result.Should().BeTrue();
        cart.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar erro quando cliente for inválido")]
    public void Should_Return_Error_When_Customer_Is_Invalid()
    {
        // Arrange
        var cart = new CustomerShoppingCart(Guid.Empty);

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "SSD",
            1,
            500
        );

        cart.AddItem(item);

        // Act
        var result = cart.IsValid();

        // Assert
        result.Should().BeFalse();

        cart.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Customer not found");
    }

    [Fact(DisplayName = "Deve retornar erro quando carrinho não possuir itens")]
    public void Should_Return_Error_When_Shopping_Cart_Has_No_Items()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        // Act
        var result = cart.IsValid();

        // Assert
        result.Should().BeFalse();

        cart.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "The shopping cart does not have any items");
    }

    [Fact(DisplayName = "Deve retornar erro quando total do carrinho for inválido")]
    public void Should_Return_Error_When_Shopping_Cart_Total_Is_Invalid()
    {
        // Arrange
        var cart = CustomerShoppingCartFactory.Create();

        var item = CartItemFactory.Create(
            Guid.NewGuid(),
            "Produto",
            0,
            100
        );

        cart.Items.Add(item);

        // Act
        var result = cart.IsValid();

        // Assert
        result.Should().BeFalse();

        cart.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "The shopping cart total amount should be greater than 0");
    }
}