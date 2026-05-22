using FluentAssertions;
using NSE.Order.Application.DTOs;
using NSE.Order.UnitTests.Factories;

namespace NSE.Order.UnitTests.Commands;

public class AddOrderCommandTests
{
    [Fact(DisplayName = "Deve validar comando válido")]
    public void Should_Validate_Valid_Command()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeTrue();
        command.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar erro quando cliente for inválido")]
    public void Should_Return_Error_When_Customer_Is_Invalid()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.CustomerId = Guid.Empty;

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Invalid customer id");
    }

    [Fact(DisplayName = "Deve retornar erro quando pedido não possuir itens")]
    public void Should_Return_Error_When_Order_Has_No_Items()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.OrderItems = new List<OrderItemDto>();

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "The order needs to have at least 1 item");
    }

    [Fact(DisplayName = "Deve retornar erro quando valor do pedido for inválido")]
    public void Should_Return_Error_When_Order_Amount_Is_Invalid()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.Amount = 0;

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Invalid order amount");
    }

    [Fact(DisplayName = "Deve retornar erro quando cartão for inválido")]
    public void Should_Return_Error_When_Credit_Card_Is_Invalid()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.CardNumber = "123";

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Invalid credit card");
    }

    [Fact(DisplayName = "Deve retornar erro quando código de segurança for inválido")]
    public void Should_Return_Error_When_Security_Code_Is_Invalid()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.SecurityCode = "1";

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "The security code must have at least 3 or 4 numbers.");
    }
}