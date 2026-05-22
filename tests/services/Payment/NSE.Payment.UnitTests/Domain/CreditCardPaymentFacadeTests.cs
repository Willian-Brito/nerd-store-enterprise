using FluentAssertions;
using NSE.Payment.API.Facade;

namespace NSE.Payment.UnitTests.Domain;

public class CreditCardPaymentFacadeTests
{
    [Fact(DisplayName = "Deve converter transação do NerdPay para domínio corretamente")]
    public void Should_Convert_NerdPay_Transaction_To_Domain_Correctly()
    {
        // Arrange
        var transaction = new NerdPay.Transaction(null)
        {
            Status = NerdPay.TransactionStatus.Authorized,
            Amount = 150,
            CardBrand = "Visa",
            AuthorizationCode = "123",
            Cost = 5,
            Nsu = "999",
            Tid = "TID-1"
        };

        // Act
        var result = CreditCardPaymentFacade.ToTransaction(transaction);

        // Assert
        result.TransactionStatus.Should().Be(API.Models.TransactionStatus.Authorized);
        result.Amount.Should().Be(150);
        result.CreditCardCompany.Should().Be("Visa");
        result.AuthorizationCode.Should().Be("123");
        result.NSU.Should().Be("999");
        result.TID.Should().Be("TID-1");
    }

    [Fact(DisplayName = "Deve converter transação de domínio para NerdPay corretamente")]
    public void Should_Convert_Domain_Transaction_To_NerdPay_Correctly()
    {
        // Arrange
        var service = new NerdPay.NerdPayService("key", "secret");

        var transaction = new API.Models.Transaction
        {
            TransactionStatus = API.Models.TransactionStatus.Paid,
            Amount = 200,
            CreditCardCompany = "Mastercard",
            AuthorizationCode = "ABC123",
            TransactionCost = 10,
            NSU = "123456",
            TID = "TID-999"
        };

        // Act
        var result = CreditCardPaymentFacade.ToTransactionPay(
            transaction,
            service
        );

        // Assert
        result.Status.Should().Be(NerdPay.TransactionStatus.Paid);
        result.Amount.Should().Be(200);
        result.CardBrand.Should().Be("Mastercard");
        result.AuthorizationCode.Should().Be("ABC123");
        result.Nsu.Should().Be("123456");
        result.Tid.Should().Be("TID-999");
    }
}