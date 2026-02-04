using Microsoft.Extensions.Options;
using NerdPay = NSE.Payment.NerdPay;
using Models = NSE.Payment.API.Models;

namespace NSE.Payment.API.Facade;

public class CreditCardPaymentFacade: IPaymentFacade
{
    private readonly PaymentConfig _paymentConfig;

    public CreditCardPaymentFacade(IOptions<PaymentConfig> paymentConfig)
    {
        _paymentConfig = paymentConfig.Value;
    }
    
    public async Task<Models.Transaction> AuthorizePayment(Models.Payment payment)
    {
        var nerdPayService = new NerdPay.NerdPayService(
            _paymentConfig.DefaultApiKey,
            _paymentConfig.DefaultEncryptionKey
        );

        var cardHashGen = new NerdPay.CardHash(nerdPayService)
        {
            CardNumber = payment.CreditCard.CardNumber,
            CardHolderName = payment.CreditCard.Holder,
            CardExpirationDate = payment.CreditCard.ExpirationDate,
            CardCvv = payment.CreditCard.SecurityCode
        };
        var cardHash = cardHashGen.Generate();

        var transaction = new NerdPay.Transaction(nerdPayService)
        {
            CardHash = cardHash,
            CardNumber = payment.CreditCard.CardNumber,
            CardHolderName = payment.CreditCard.Holder,
            CardExpirationDate = payment.CreditCard.ExpirationDate,
            CardCvv = payment.CreditCard.SecurityCode,
            PaymentMethod = NerdPay.PaymentMethod.CreditCard,
            Amount = payment.Amount
        };

        return ToTransaction(await transaction.AuthorizeCardTransaction());
    }

    public async Task<Models.Transaction> CapturePayment(Models.Transaction transaction)
    {
        var nerdPayService = new NerdPay.NerdPayService(
            _paymentConfig.DefaultApiKey,
            _paymentConfig.DefaultEncryptionKey
        );

        var tr = ToTransactionPay(transaction, nerdPayService);

        return ToTransaction(await tr.CaptureCardTransaction());
    }

    public async Task<Models.Transaction> CancelAuthorization(Models.Transaction transaction)
    {
        var nerdPayService = new NerdPay.NerdPayService(
            _paymentConfig.DefaultApiKey,
            _paymentConfig.DefaultEncryptionKey
        );
        var tr = ToTransactionPay(transaction, nerdPayService);
        
        return ToTransaction(await tr.CancelAuthorization());
    }

    public static Models.Transaction ToTransaction(NerdPay.Transaction transaction)
    {
        return new Models.Transaction
        {
            Id = Guid.NewGuid(),
            TransactionStatus = (Models.TransactionStatus)transaction.Status,
            Amount = transaction.Amount,
            CreditCardCompany = transaction.CardBrand,
            AuthorizationCode = transaction.AuthorizationCode,
            TransactionCost = transaction.Cost,
            TransactionDate = transaction.TransactionDate,
            NSU = transaction.Nsu,
            TID = transaction.Tid
        };
    }

    public static NerdPay.Transaction ToTransactionPay(
        Models.Transaction transaction, 
        NerdPay.NerdPayService nerdPayService
    )
    {
        return new NerdPay.Transaction(nerdPayService)
        {
            Status = (NerdPay.TransactionStatus)transaction.TransactionStatus,
            Amount = transaction.Amount,
            CardBrand = transaction.CreditCardCompany,
            AuthorizationCode = transaction.AuthorizationCode,
            Cost = transaction.TransactionCost,
            Nsu = transaction.NSU,
            Tid = transaction.TID
        };
    }
}