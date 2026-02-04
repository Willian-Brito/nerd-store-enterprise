using NSE.Core.Messages.Base;

namespace NSE.Payment.API.Services;

public interface IPaymentService
{
    Task<ResponseMessage> AuthorizeTransaction(Models.Payment payment);
    Task<ResponseMessage> CaptureTransaction(Guid orderId);
    Task<ResponseMessage> CancelTransaction(Guid orderId);
}