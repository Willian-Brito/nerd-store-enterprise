using Microsoft.AspNetCore.Mvc;
using NSE.Core.Messages.Integration;
using NSE.Payment.API.Services;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Payment.API.Controllers;

public class PaymentController : MainController
{
    [HttpPost("api/payment/order-initiated")]
    public async Task<IActionResult> PostAsync(
        OrderInitiatedIntegrationEvent orderInitiatedIntegrationEvent,
        [FromServices] IPaymentService paymentService
    )
    {
        var payment = GetPaymentFrom(orderInitiatedIntegrationEvent);
        var response = await paymentService.AuthorizeTransaction(payment);
        
        return Ok(response);
    }

    private static Models.Payment GetPaymentFrom(OrderInitiatedIntegrationEvent orderInitiatedIntegrationEvent)
    {
        return new Models.Payment
        {
            OrderId = orderInitiatedIntegrationEvent.OrderId,
            PaymentType = (Models.PaymentType)orderInitiatedIntegrationEvent.PaymentType,
            Amount = orderInitiatedIntegrationEvent.Amount,
            CreditCard = new Models.CreditCard(
                orderInitiatedIntegrationEvent.Holder, 
                orderInitiatedIntegrationEvent.CardNumber, 
                orderInitiatedIntegrationEvent.ExpirationDate, 
                orderInitiatedIntegrationEvent.SecurityCode
            )
        };
    }
}