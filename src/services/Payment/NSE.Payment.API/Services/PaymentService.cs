using FluentValidation.Results;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Base;
using NSE.Payment.API.Facade;
using NSE.Payment.API.Models;

namespace NSE.Payment.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentFacade _paymentFacade;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(
        IPaymentFacade paymentFacade,
        IPaymentRepository paymentRepository
    )
    {
        _paymentFacade = paymentFacade;
        _paymentRepository = paymentRepository;
    }
    
    public async Task<ResponseMessage> AuthorizeTransaction(Models.Payment payment)
    {
        var transaction = await _paymentFacade.AuthorizePayment(payment);
        var validationResult = new ValidationResult();

        if (transaction.TransactionStatus != TransactionStatus.Authorized)
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                "Payment refused, please contact your card operator")
            );

            return new ResponseMessage(validationResult);
        }

        payment.AddTransaction(transaction);
        _paymentRepository.AddPayment(payment);

        if (!await _paymentRepository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                "There was an error while making the payment.")
            );

            // Canceling the payment on the service
            await CancelTransaction(payment.OrderId);

            return new ResponseMessage(validationResult);
        }

        return new ResponseMessage(validationResult);
    }

    public async Task<ResponseMessage> CaptureTransaction(Guid orderId)
    {
        var transactions = await _paymentRepository.GetTransactionsByOrderId(orderId);
        var authorizedTransaction = transactions?
            .FirstOrDefault(t => t.TransactionStatus == TransactionStatus.Authorized);
        var validationResult = new ValidationResult();

        if (authorizedTransaction == null) throw new DomainException($"Transaction not found for order {orderId}");

        var transaction = await _paymentFacade.CapturePayment(authorizedTransaction);

        if (transaction.TransactionStatus != TransactionStatus.Paid)
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                $"Unable to capture order payment {orderId}")
            );

            return new ResponseMessage(validationResult);
        }

        transaction.PaymentId = authorizedTransaction.PaymentId;
        _paymentRepository.AddTransaction(transaction);

        if (!await _paymentRepository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                $"It was not possible to persist the capture of the payment of the order {orderId}")
            );

            return new ResponseMessage(validationResult);
        }

        return new ResponseMessage(validationResult);
    }

    public async Task<ResponseMessage> CancelTransaction(Guid orderId)
    {
        var transactions = await _paymentRepository.GetTransactionsByOrderId(orderId);
        var authorizedTransaction =
            transactions?.FirstOrDefault(t => t.TransactionStatus == TransactionStatus.Authorized);
        var validationResult = new ValidationResult();

        if (authorizedTransaction == null) throw new DomainException($"Transaction not found for order {orderId}");

        var transaction = await _paymentFacade.CancelAuthorization(authorizedTransaction);

        if (transaction.TransactionStatus != TransactionStatus.Canceled)
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                $"Unable to cancel order payment {orderId}")
            );

            return new ResponseMessage(validationResult);
        }

        transaction.PaymentId = authorizedTransaction.PaymentId;
        _paymentRepository.AddTransaction(transaction);

        if (!await _paymentRepository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Payment",
                $"It was not possible to persist the cancellation of the order payment {orderId}")
            );

            return new ResponseMessage(validationResult);
        }

        return new ResponseMessage(validationResult);
    }
}