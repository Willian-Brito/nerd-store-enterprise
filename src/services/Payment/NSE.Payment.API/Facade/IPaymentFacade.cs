using Models = NSE.Payment.API.Models;

namespace NSE.Payment.API.Facade;

public interface IPaymentFacade
{
    Task<Models.Transaction> AuthorizePayment(Models.Payment payment);
    Task<Models.Transaction> CapturePayment(Models.Transaction transaction);
    Task<Models.Transaction> CancelAuthorization(Models.Transaction transaction);
}