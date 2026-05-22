using FluentAssertions;

namespace NSE.Customer.UnitTests.Domain;

public class CustomerTests
{
    [Fact(DisplayName = "Deve criar cliente válido")]
    public void Should_Create_Valid_Customer()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var customer = new API.Models.Entities.Customer(
            customerId,
            "Willian Brito",
            "willian@email.com",
            "12345678900"
        );

        // Assert
        customer.Should().NotBeNull();
        customer.Id.Should().Be(customerId);
        customer.Name.Should().Be("Willian Brito");
        customer.Email.Address.Should().Be("willian@email.com");
        customer.SocialNumber.Should().Be("12345678900");
        customer.Deleted.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve alterar email do cliente")]
    public void Should_Change_Customer_Email()
    {
        // Arrange
        var customer = new API.Models.Entities.Customer(
            Guid.NewGuid(),
            "Willian Brito",
            "old@email.com",
            "12345678900"
        );

        // Act
        customer.ChangeEmail("new@email.com");

        // Assert
        customer.Email.Address.Should().Be("new@email.com");
    }

    [Fact(DisplayName = "Deve definir endereço para cliente")]
    public void Should_Set_Address_For_Customer()
    {
        // Arrange
        var customer = new API.Models.Entities.Customer(
            Guid.NewGuid(),
            "Willian Brito",
            "willian@email.com",
            "12345678900"
        );

        var address = new API.Models.Entities.Address(
            "Rua XPTO",
            "100",
            "Complemento",
            "Centro",
            "17690000",
            "Bastos",
            "SP",
            customer.Id
        );

        // Act
        customer.SetAddress(address);

        // Assert
        customer.Address.Should().NotBeNull();
        customer.Address.StreetAddress.Should().Be("Rua XPTO");
        customer.Address.BuildingNumber.Should().Be("100");
        customer.Address.City.Should().Be("Bastos");
    }

    [Theory(DisplayName = "Deve lançar exceção ao criar email inválido")]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("teste@")]
    [InlineData("@teste.com")]
    public void Should_Throw_Exception_When_Email_Is_Invalid(string invalidEmail)
    {
        // Act
        Action action = () => new API.Models.Entities.Customer(
            Guid.NewGuid(),
            "Willian Brito",
            invalidEmail,
            "12345678900"
        );

        // Assert
        action.Should().Throw<Exception>();
    }

    [Theory(DisplayName = "Deve lançar exceção ao alterar para email inválido")]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("teste@")]
    public void Should_Throw_Exception_When_Changing_To_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var customer = new API.Models.Entities.Customer(
            Guid.NewGuid(),
            "Willian Brito",
            "valid@email.com",
            "12345678900"
        );

        // Act
        Action action = () => customer.ChangeEmail(invalidEmail);

        // Assert
        action.Should().Throw<Exception>();
    }
}