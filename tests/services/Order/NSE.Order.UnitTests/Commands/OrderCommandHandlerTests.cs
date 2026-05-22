using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NSE.Core.Data;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.MessageBroker.Abstractions;
using NSE.Order.Application.Commands;
using NSE.Order.Domain.Entities.Vouchers;
using NSE.Order.Domain.Interfaces;
using NSE.Order.UnitTests.Factories;
using NSE.WebAPI.Core.Http;

namespace NSE.Order.UnitTests.Commands;

public class OrderCommandHandlerTests
{
    private readonly Mock<IVoucherRepository> _voucherRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IRpcBus> _rpcBusMock;
    private readonly Mock<IRestClient> _restClientMock;

    private readonly OrderCommandHandler _handler;

    public OrderCommandHandlerTests()
    {
        _voucherRepositoryMock = new Mock<IVoucherRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _rpcBusMock = new Mock<IRpcBus>();
        _restClientMock = new Mock<IRestClient>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        unitOfWorkMock
            .Setup(x => x.Commit())
            .ReturnsAsync(true);

        _orderRepositoryMock
            .Setup(x => x.UnitOfWork)
            .Returns(unitOfWorkMock.Object);

        _handler = new OrderCommandHandler(
            _voucherRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _rpcBusMock.Object,
            _restClientMock.Object
        );
    }

    [Fact(DisplayName = "Deve adicionar pedido com sucesso")]
    public async Task Should_Add_Order_Successfully()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();

        _rpcBusMock
            .Setup(x =>
                x.RequestAsync<OrderInitiatedIntegrationEvent, ResponseMessage>(
                    It.IsAny<OrderInitiatedIntegrationEvent>()))
            .ReturnsAsync(new ResponseMessage(new ValidationResult()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();

        _orderRepositoryMock.Verify(
            x => x.Add(It.IsAny<Order.Domain.Entities.Orders.Order>()),
            Times.Once
        );
    }

    [Fact(DisplayName = "Deve retornar erro quando voucher não existir")]
    public async Task Should_Return_Error_When_Voucher_Does_Not_Exist()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.HasVoucher = true;
        command.Voucher = "PROMO10";

        _voucherRepositoryMock
            .Setup(x => x.GetVoucherByCode(It.IsAny<string>()))
            .ReturnsAsync((Voucher)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.ErrorMessage == "Voucher not found!");
    }

    [Fact(DisplayName = "Deve retornar erro quando voucher estiver expirado")]
    public async Task Should_Return_Error_When_Voucher_Is_Expired()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.HasVoucher = true;
        command.Voucher = "PROMO10";

        var voucher = new Voucher(
            "PROMO10",
            10,
            null,
            10,
            VoucherDiscountType.Percentage,
            DateTime.UtcNow.AddDays(-1)
        );

        _voucherRepositoryMock
            .Setup(x => x.GetVoucherByCode(It.IsAny<string>()))
            .ReturnsAsync(voucher);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.ErrorMessage == "Este voucher está expirado");
    }

    [Fact(DisplayName = "Deve retornar erro quando pagamento falhar")]
    public async Task Should_Return_Error_When_Payment_Fails()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();

        var validationResult = new ValidationResult();
        validationResult.Errors.Add(
            new ValidationFailure("Payment", "Pagamento recusado")
        );

        _rpcBusMock
            .Setup(x =>
                x.RequestAsync<OrderInitiatedIntegrationEvent, ResponseMessage>(
                    It.IsAny<OrderInitiatedIntegrationEvent>()))
            .ReturnsAsync(new ResponseMessage(validationResult));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x => x.ErrorMessage == "Pagamento recusado");
    }

    [Fact(DisplayName = "Deve atualizar voucher quando utilizado")]
    public async Task Should_Update_Voucher_When_Used()
    {
        // Arrange
        var command = AddOrderCommandFactory.Create();
        command.HasVoucher = true;
        command.Voucher = "PROMO10";
        command.Amount = 90;
        command.Discount = 10;

        var voucher = new Voucher(
            "PROMO10",
            10,
            null,
            10,
            VoucherDiscountType.Percentage,
            DateTime.UtcNow.AddDays(10)
        );

        _voucherRepositoryMock
            .Setup(x => x.GetVoucherByCode(It.IsAny<string>()))
            .ReturnsAsync(voucher);

        _rpcBusMock
            .Setup(x =>
                x.RequestAsync<OrderInitiatedIntegrationEvent, ResponseMessage>(
                    It.IsAny<OrderInitiatedIntegrationEvent>()))
            .ReturnsAsync(new ResponseMessage(new ValidationResult()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();

        _voucherRepositoryMock.Verify(
            x => x.Update(It.IsAny<Voucher>()),
            Times.Once
        );
    }
}