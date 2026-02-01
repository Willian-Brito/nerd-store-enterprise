using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Core.Messages.Integration;
using NSE.Order.Application.Queries.Order;
using NSE.Queue.Abstractions;

namespace NSE.Order.Application.Events;

public class OrderOrchestratorIntegrationHandler : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public OrderOrchestratorIntegrationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Order service initialized.");

        _timer = new Timer(
            ProcessOrders, 
            null, 
            TimeSpan.Zero,
            TimeSpan.FromSeconds(15)
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Order service finished.");

        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void ProcessOrders(object state)
    {
        using var scope = _serviceProvider.CreateScope();
        var orderQueries = scope.ServiceProvider.GetRequiredService<IOrderQueries>();
        var order = await orderQueries.GetAuthorizedOrders();

        if (order == null) return;

        var authorizedOrder = new OrderAuthorizedIntegrationEvent(
            order.CustomerId, 
            order.Id,
            order.OrderItems.ToDictionary(p => p.ProductId, p => p.Quantity)
        );
        
        var queue = scope.ServiceProvider.GetRequiredService<IQueue>();
        await queue.PublishAsync(authorizedOrder);

        Console.WriteLine($"Order ID: {order.Id} was sent to lower at stock.");
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}