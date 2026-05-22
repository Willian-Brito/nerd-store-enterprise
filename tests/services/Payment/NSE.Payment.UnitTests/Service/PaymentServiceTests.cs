using FluentAssertions;
using Moq;
using NSE.Core.DomainObjects;
using NSE.Payment.API.Facade;
using NSE.Payment.API.Models;
using NSE.Payment.API.Services;
using NSE.Payment.UnitTests.Factories;

namespace NSE.Payment.UnitTests.Service;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentFacade> _paymentFacadeMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;

    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentFacadeMock = new Mock<IPaymentFacade>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();

        _paymentService = new PaymentService(
            _paymentFacadeMock.Object,
            _paymentRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "Deve autorizar transação com sucesso")]
    public async Task Should_Authorize_Transaction_Successfully()
    {
        // Arrange
        var payment = PaymentFactory.Create();

        var transaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Authorized,
            Amount = payment.Amount
        };

        _paymentFacadeMock
            .Setup(x => x.AuthorizePayment(It.IsAny<API.Models.Payment>()))
            .ReturnsAsync(transaction);

        _paymentRepositoryMock
            .Setup(x => x.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _paymentService.AuthorizeTransaction(payment);

        // Assert
        result.ValidationResult.IsValid.Should().BeTrue();

        _paymentRepositoryMock.Verify(
            x => x.AddPayment(It.IsAny<API.Models.Payment>()),
            Times.Once
        );
    }

    [Fact(DisplayName = "Deve retornar erro quando pagamento for recusado")]
    public async Task Should_Return_Error_When_Payment_Is_Refused()
    {
        // Arrange
        var payment = PaymentFactory.Create();

        var transaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Denied
        };

        _paymentFacadeMock
            .Setup(x => x.AuthorizePayment(It.IsAny<API.Models.Payment>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _paymentService.AuthorizeTransaction(payment);

        // Assert
        result.ValidationResult.IsValid.Should().BeFalse();

        result.ValidationResult.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "Payment refused, please contact your card operator");
    }

    [Fact(DisplayName = "Deve cancelar transação quando commit falhar")]
    public async Task Should_Cancel_Transaction_When_Commit_Fails()
    {
        // Arrange
        var payment = PaymentFactory.Create();

        var authorizedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            TransactionStatus = TransactionStatus.Authorized
        };

        var canceledTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PaymentId = authorizedTransaction.PaymentId,
            TransactionStatus = TransactionStatus.Canceled
        };

        _paymentFacadeMock
            .Setup(x => x.AuthorizePayment(It.IsAny<API.Models.Payment>()))
            .ReturnsAsync(authorizedTransaction);

        _paymentRepositoryMock
            .SetupSequence(x => x.UnitOfWork.Commit())
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        _paymentRepositoryMock
            .Setup(x => x.GetTransactionsByOrderId(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Transaction> { authorizedTransaction });

        _paymentFacadeMock
            .Setup(x => x.CancelAuthorization(It.IsAny<Transaction>()))
            .ReturnsAsync(canceledTransaction);

        // Act
        var result = await _paymentService.AuthorizeTransaction(payment);

        // Assert
        result.ValidationResult.IsValid.Should().BeFalse();

        result.ValidationResult.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "There was an error while making the payment.");
    }

    [Fact(DisplayName = "Deve capturar pagamento com sucesso")]
    public async Task Should_Capture_Payment_Successfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var authorizedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            TransactionStatus = TransactionStatus.Authorized
        };

        var paidTransaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Paid
        };

        _paymentRepositoryMock
            .Setup(x => x.GetTransactionsByOrderId(orderId))
            .ReturnsAsync(new List<Transaction> { authorizedTransaction });

        _paymentFacadeMock
            .Setup(x => x.CapturePayment(It.IsAny<Transaction>()))
            .ReturnsAsync(paidTransaction);

        _paymentRepositoryMock
            .Setup(x => x.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _paymentService.CaptureTransaction(orderId);

        // Assert
        result.ValidationResult.IsValid.Should().BeTrue();

        _paymentRepositoryMock.Verify(
            x => x.AddTransaction(It.IsAny<Transaction>()),
            Times.Once
        );
    }

    [Fact(DisplayName = "Deve retornar erro quando captura falhar")]
    public async Task Should_Return_Error_When_Capture_Fails()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var authorizedTransaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Authorized
        };

        var deniedTransaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Denied
        };

        _paymentRepositoryMock
            .Setup(x => x.GetTransactionsByOrderId(orderId))
            .ReturnsAsync(new List<Transaction> { authorizedTransaction });

        _paymentFacadeMock
            .Setup(x => x.CapturePayment(It.IsAny<Transaction>()))
            .ReturnsAsync(deniedTransaction);

        // Act
        var result = await _paymentService.CaptureTransaction(orderId);

        // Assert
        result.ValidationResult.IsValid.Should().BeFalse();

        result.ValidationResult.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == $"Unable to capture order payment {orderId}");
    }

    [Fact(DisplayName = "Deve cancelar pagamento com sucesso")]
    public async Task Should_Cancel_Payment_Successfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var authorizedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            TransactionStatus = TransactionStatus.Authorized
        };

        var canceledTransaction = new Transaction
        {
            TransactionStatus = TransactionStatus.Canceled
        };

        _paymentRepositoryMock
            .Setup(x => x.GetTransactionsByOrderId(orderId))
            .ReturnsAsync(new List<Transaction> { authorizedTransaction });

        _paymentFacadeMock
            .Setup(x => x.CancelAuthorization(It.IsAny<Transaction>()))
            .ReturnsAsync(canceledTransaction);

        _paymentRepositoryMock
            .Setup(x => x.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _paymentService.CancelTransaction(orderId);

        // Assert
        result.ValidationResult.IsValid.Should().BeTrue();

        _paymentRepositoryMock.Verify(
            x => x.AddTransaction(It.IsAny<Transaction>()),
            Times.Once
        );
    }

    [Fact(DisplayName = "Deve lançar exceção quando transação não existir")]
    public async Task Should_Throw_Exception_When_Transaction_Does_Not_Exist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _paymentRepositoryMock
            .Setup(x => x.GetTransactionsByOrderId(orderId))
            .ReturnsAsync(new List<Transaction>());

        // Act
        Func<Task> act = async () =>
            await _paymentService.CancelTransaction(orderId);

        // Assert
        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage($"Transaction not found for order {orderId}");
    }
}