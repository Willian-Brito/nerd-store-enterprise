using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSE.Core.Messages.Integration;
using NSE.Order.Application.Queries.Order;
using NSE.Queue.Abstractions;

namespace NSE.Order.Application.Services;

public class OrderOrchestratorIntegrationJob : IHostedService, IDisposable
{
    private readonly ILogger<OrderOrchestratorIntegrationJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public OrderOrchestratorIntegrationJob(
        ILogger<OrderOrchestratorIntegrationJob> logger,
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order service initialized.");

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
        _logger.LogInformation("Order service finished.");

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

        _logger.LogInformation($"Order ID: {order.Id} was sent to lower at stock.");
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}