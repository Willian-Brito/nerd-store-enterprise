using FluentAssertions;
using NSE.Customer.API.Application.Commands;

namespace NSE.Customer.UnitTests.Commands;

public class AddAddressCommandTests
{
    [Fact(DisplayName = "Deve validar o comando de endereço válido")]
    public void Should_Validate_Valid_Command()
    {
        // Arrange
        var command = new AddAddressCommand(
            Guid.NewGuid(),
            "Street 1",
            "100",
            "Apt 10",
            "Center",
            "12345-000",
            "São Paulo",
            "SP"
        );

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve invalidar a rua vazia")]
    public void Should_Invalidate_Empty_Street()
    {
        var command = new AddAddressCommand(
            Guid.NewGuid(),
            "",
            "100",
            "Apt 10",
            "Center",
            "12345-000",
            "São Paulo",
            "SP"
        );

        command.IsValid().Should().BeFalse();
    }

    [Fact(DisplayName = "Deve invalidar a CEP vazio")]
    public void Should_Invalidate_Empty_ZipCode()
    {
        var command = new AddAddressCommand(
            Guid.NewGuid(),
            "Street 1",
            "100",
            "Apt 10",
            "Center",
            "",
            "São Paulo",
            "SP"
        );

        command.IsValid().Should().BeFalse();
    }
}