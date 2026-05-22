using FluentAssertions;
using NSE.Customer.API.Application.Commands;

namespace NSE.Customer.UnitTests.Commands;

public class NewCustomerCommandTests
{
    [Fact(DisplayName = "Deve validar comando")]
    public void Should_Validate_Valid_Command()
    {
        // Arrange
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "Willian",
            "willian@email.com",
            "12345678900"
        );

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve invalidar um ID de cliente vazio")]
    public void Should_Invalidate_Empty_Id()
    {
        // Arrange
        var command = new NewCustomerCommand(
            Guid.Empty,
            "Willian",
            "willian@email.com",
            "12345678900"
        );

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors
            .Should()
            .Contain(x => x.ErrorMessage == "Invalid customer id");
    }

    [Fact(DisplayName = "Deve invalidar um nome de cliente vazio")]
    public void Should_Invalidate_Empty_Name()
    {
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "",
            "willian@email.com",
            "12345678900"
        );

        command.IsValid().Should().BeFalse();
    }

    [Fact(DisplayName = "Deve invalidar um email inválido")]
    public void Should_Invalidate_Invalid_Email()
    {
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "Willian",
            "invalid-email",
            "12345678900"
        );

        command.IsValid().Should().BeFalse();
    }

    [Fact(DisplayName = "Deve invalidar um CPF vazio")]
    public void Should_Invalidate_Empty_SocialNumber()
    {
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "Willian",
            "willian@email.com",
            ""
        );

        command.IsValid().Should().BeFalse();
    }
}