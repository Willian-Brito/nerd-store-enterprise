using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.Order.Domain.Interfaces;
using NSE.Queue.Abstractions;

namespace NSE.Order.Infra.Jobs;

public class OrderIntegrationJob : BackgroundService
{
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public OrderIntegrationJob(IServiceProvider serviceProvider, IQueue queue)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }
    
    private void SetSubscribers()
    {
        _queue.SubscribeAsync<OrderCanceledIntegrationEvent>(
            "PedidoCancelado",
            async request => await CancelOrder(request)
        );

        _queue.SubscribeAsync<OrderPaidIntegrationEvent>(
            "PedidoPago",
            async request => await FinishOrder(request)
        );
    }
    
    private async Task CancelOrder(OrderCanceledIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();

        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var order = await orderRepository.GetById(message.OrderId);
        order.Cancel();

        orderRepository.Update(order);

        if (!await orderRepository.UnitOfWork.Commit())
            throw new DomainException($"Problems while trying to cancel order {message.OrderId}");
    }

    private async Task FinishOrder(OrderPaidIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var order = await orderRepository.GetById(message.OrderId);
        order.Finish();

        orderRepository.Update(order);

        if (!await orderRepository.UnitOfWork.Commit())
            throw new DomainException($"Problems found trying to finish o order {message.OrderId}");
    }
}