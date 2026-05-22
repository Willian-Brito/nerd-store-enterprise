using FluentAssertions;
using Moq;
using NSE.Core.Data;
using NSE.Customer.API.Application.Commands;
using NSE.Customer.API.Data.Models.Interfaces;
using NSE.Customer.API.Models.Entities;

namespace NSE.Customer.UnitTests.Commands;

public class CustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly CustomerCommandHandler _handler;

    public CustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _customerRepositoryMock
            .Setup(x => x.UnitOfWork)
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(x => x.Commit())
            .ReturnsAsync(true);

        _handler = new CustomerCommandHandler(
            _customerRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "Deve criar o cliente com sucesso")]
    public async Task Should_Create_Customer_Successfully()
    {
        // Arrange
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "Willian",
            "willian@email.com",
            "12345678900"
        );

        _customerRepositoryMock
            .Setup(x => x.GetBySocialNumber(It.IsAny<string>()))
            .ReturnsAsync((API.Models.Entities.Customer)null);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.IsValid.Should().BeTrue();

        _customerRepositoryMock.Verify(
            x => x.Add(It.IsAny<API.Models.Entities.Customer>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.Commit(),
            Times.Once
        );
    }

    [Fact(DisplayName = "Não deve criar clientes duplicados")]
    public async Task Should_Not_Create_Duplicated_Customer()
    {
        // Arrange
        var command = new NewCustomerCommand(
            Guid.NewGuid(),
            "Willian",
            "willian@email.com",
            "12345678900"
        );

        var customer = new API.Models.Entities.Customer(
            Guid.NewGuid(),
            "Existing",
            "existing@email.com",
            "12345678900"
        );

        _customerRepositoryMock
            .Setup(x => x.GetBySocialNumber(It.IsAny<string>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.IsValid.Should().BeFalse();

        _customerRepositoryMock.Verify(
            x => x.Add(It.IsAny<API.Models.Entities.Customer>()),
            Times.Never
        );
    }

    [Fact(DisplayName = "Não deve persistir comando inválido")]
    public async Task Should_Not_Persist_Invalid_Command()
    {
        // Arrange
        var command = new NewCustomerCommand(
            Guid.Empty,
            "",
            "invalid-email",
            ""
        );

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.IsValid.Should().BeFalse();

        _customerRepositoryMock.Verify(
            x => x.Add(It.IsAny<API.Models.Entities.Customer>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            x => x.Commit(),
            Times.Never
        );
    }

    [Fact(DisplayName = "Deve adicionar o endereço com sucesso")]
    public async Task Should_Add_Address_Successfully()
    {
        // Arrange
        var command = new AddAddressCommand(
            Guid.NewGuid(),
            "Street",
            "100",
            "",
            "Center",
            "12345",
            "São Paulo",
            "SP"
        );

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.IsValid.Should().BeTrue();

        _customerRepositoryMock.Verify(
            x => x.AddAddress(It.IsAny<Address>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.Commit(),
            Times.Once
        );
    }

    [Fact(DisplayName = "Não deve adicionar endereços inválidos")]
    public async Task Should_Not_Add_Invalid_Address()
    {
        // Arrange
        var command = new AddAddressCommand(
            Guid.NewGuid(),
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.IsValid.Should().BeFalse();

        _customerRepositoryMock.Verify(
            x => x.AddAddress(It.IsAny<Address>()),
            Times.Never
        );
    }
}