using FluentAssertions;
using NSE.Order.Domain.Entities.Orders;
using NSE.Order.Domain.Entities.Vouchers;
using NSE.Order.UnitTests.Factories;

namespace NSE.Order.UnitTests.Domain;

public class OrderTests
{
    [Fact(DisplayName = "Deve criar o pedido com sucesso")]
    public void Should_Create_Order_Successfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var items = new List<OrderItem>
        {
            new OrderItem(
                productId: Guid.NewGuid(),
                productName: "Mouse",
                quantity: 2,
                price: 100
            )
        };

        // Act
        var order = new Order.Domain.Entities.Orders.Order(
            customerId,
            200,
            items
        );

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(customerId);
        order.Amount.Should().Be(200);
        order.OrderItems.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Deve autorizar o pedido")]
    public void Should_Authorize_Order()
    {
        // Arrange
        var order = OrderFactory.Create();

        // Act
        order.Authorize();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Authorized);
    }

    [Fact(DisplayName = "Deve cancelar o pedido")]
    public void Should_Cancel_Order()
    {
        // Arrange
        var order = OrderFactory.Create();

        // Act
        order.Cancel();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Canceled);
    }

    [Fact(DisplayName = "Deve finalizar o pedido")]
    public void Should_Finish_Order()
    {
        // Arrange
        var order = OrderFactory.Create();

        // Act
        order.Finish();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Paid);
    }

    [Fact(DisplayName = "Deve associar cupom de desconto ao pedido")]
    public void Should_Associate_Voucher()
    {
        // Arrange
        var order = OrderFactory.Create();

        var voucher = new Voucher(
            code: "OFF-10", 
            percentage: 0, 
            discount: 10M, 
            quantity: 10, 
            discountType: VoucherDiscountType.Value,
            expirationDate: DateTime.Now.AddDays(5)
        );

        // Act
        order.AssociateVoucher(voucher);

        // Assert
        order.HasVoucher.Should().BeTrue();
        order.VoucherId.Should().Be(voucher.Id);
        order.Voucher.Should().Be(voucher);
    }

    [Fact(DisplayName = "Deve adicionar endereço ao pedido")]
    public void Should_Set_Address()
    {
        // Arrange
        var order = OrderFactory.Create();

        var address = new Address
        {
            StreetAddress = "Street",
            BuildingNumber = "100",
            SecondaryAddress = "",
            Neighborhood = "Center",
            ZipCode = "12345-000",
            City = "São Paulo",
            State = "SP"
        };

        // Act
        order.SetAddress(address);

        // Assert
        order.Address.Should().NotBeNull();
        order.Address.City.Should().Be("São Paulo");
    }

    [Fact(DisplayName = "Deve calcular o valor do pedido sem cupom")]
    public void Should_Calculate_Order_Amount_Without_Voucher()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new OrderItem(
                productId: Guid.NewGuid(),
                productName: "Mouse",
                quantity: 2,
                price: 100M
            ),
            new OrderItem(
                productId: Guid.NewGuid(),
                productName: "Keuboard",
                quantity: 1,
                price: 300M
            ),
        };

        var order = new Order.Domain.Entities.Orders.Order(
            Guid.NewGuid(),
            0,
            items
        );

        // Act
        order.CalculateOrderAmount();

        // Assert
        order.Amount.Should().Be(500);
        order.Discount.Should().Be(0);
    }

    [Fact(DisplayName = "Deve aplicar cupom de desconto em percentual")]
    public void Should_Apply_Percentage_Voucher()
    {
        // Arrange
        var order = OrderFactory.Create();
        
        var voucher = new Voucher(
            code: "PEC-10", 
            percentage: 10, 
            discount: 0, 
            quantity: 20, 
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.Now.AddDays(5)
        );

        order.AssociateVoucher(voucher);

        // Act
        order.CalculateAmount();

        // Assert
        order.Discount.Should().Be(20);
        order.Amount.Should().Be(180);
    }

    [Fact(DisplayName = "Deve aplicar cupom com valor fixo")]
    public void Should_Apply_Fixed_Voucher()
    {
        // Arrange
        var order = OrderFactory.Create();

        var voucher = new Voucher(
            code: "OFF-50", 
            percentage: 0, 
            discount: 50, 
            quantity: 20, 
            discountType: VoucherDiscountType.Value,
            expirationDate: DateTime.Now.AddDays(5)
        );

        order.AssociateVoucher(voucher);

        // Act
        order.CalculateAmount();

        // Assert
        order.Discount.Should().Be(50);
        order.Amount.Should().Be(150);
    }

    [Fact(DisplayName = "Não se deve permitir valores negativos")]
    public void Should_Not_Allow_Negative_Amount()
    {
        // Arrange
        var order = OrderFactory.Create();
        
        var voucher = new Voucher(
            code: "OFF-999", 
            percentage: 0, 
            discount: 999, 
            quantity: 1, 
            discountType: VoucherDiscountType.Value,
            expirationDate: DateTime.Now.AddDays(5)
        );

        order.AssociateVoucher(voucher);

        // Act
        order.CalculateAmount();

        // Assert
        order.Amount.Should().Be(0);
    }
}