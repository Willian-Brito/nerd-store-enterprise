using NSE.Core.DomainObjects;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.Payment.API.Services;
using Models = NSE.Payment.API.Models;
using NSE.Queue.Abstractions;

namespace NSE.Payment.API.Jobs;

public class PaymentIntegrationJob : BackgroundService
{
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public PaymentIntegrationJob(
        IServiceProvider serviceProvider,
        IQueue queue
    )
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        SetSubscribers();
        return Task.CompletedTask;
    }
    
    private void SetResponder()
    {
        _queue.RespondAsync<OrderInitiatedIntegrationEvent, ResponseMessage>(
            async request => await AuthorizeTransaction(request)
        );
    }

    private void SetSubscribers()
    {
        _queue.SubscribeAsync<OrderCanceledIntegrationEvent>(
            "PedidoCancelado", 
            async request => await CancelTransaction(request)
        );

        _queue.SubscribeAsync<OrderLoweredStockIntegrationEvent>(
            "PedidoBaixadoEstoque", 
            async request => await CapturePayment(request)
        );
    }
    
    private async Task<ResponseMessage> AuthorizeTransaction(OrderInitiatedIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var payment = new Models.Payment
        {
            OrderId = message.OrderId,
            PaymentType = (Models.PaymentType)message.PaymentType,
            Amount = message.Amount,
            CreditCard = new Models.CreditCard(
                message.Holder, 
                message.CardNumber, 
                message.ExpirationDate, 
                message.SecurityCode
            )
        };

        return await paymentService.AuthorizeTransaction(payment);
    }

    private async Task CancelTransaction(OrderCanceledIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var response = await paymentService.CancelTransaction(message.OrderId);

        if (!response.ValidationResult.IsValid)
            throw new DomainException($"Failed to cancel order payment {message.OrderId}");
    }

    private async Task CapturePayment(OrderLoweredStockIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var response = await paymentService.CaptureTransaction(message.OrderId);

        if (!response.ValidationResult.IsValid)
            throw new DomainException($"Error trying to get order payment {message.OrderId}");

        await _queue.PublishAsync(new OrderPaidIntegrationEvent(message.CustomerId, message.OrderId));
    }
}