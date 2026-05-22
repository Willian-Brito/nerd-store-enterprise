using NSE.Payment.API.Models;

namespace NSE.Payment.UnitTests.Factories;

public static class PaymentFactory
{
    public static NSE.Payment.API.Models.Payment Create()
    {
        var creditCard = new CreditCard(
            "Willian Brito",
            "4111111111111111",
            "12/30",
            "123"
        );

        var payment = new NSE.Payment.API.Models.Payment
        {
            OrderId = Guid.NewGuid(),
            PaymentType = PaymentType.CreditCard,
            Amount = 100,
            CreditCard = creditCard
        };

        return payment;
    }
}