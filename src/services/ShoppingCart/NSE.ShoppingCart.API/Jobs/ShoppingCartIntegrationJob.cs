using Microsoft.EntityFrameworkCore;
using NSE.Core.Messages.Integration;
using NSE.MessageBroker.Abstractions;
using NSE.ShoppingCart.API.Data;

namespace NSE.ShoppingCart.API.Jobs;

public class ShoppingCartIntegrationJob : BackgroundService
{
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public ShoppingCartIntegrationJob(IServiceProvider serviceProvider, IQueue queue)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers(stoppingToken);
        return Task.CompletedTask;
    }
    
    private void SetSubscribers(CancellationToken stoppingToken)
    {
        _queue.SubscribeAsync<OrderDoneIntegrationEvent>(
            "OrderDone", 
            async request => await RemoveShoppingCart(request),
            stoppingToken
        );
    }
    
    private async Task RemoveShoppingCart(OrderDoneIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();

        var shoppingCart = await context.CustomerShoppingCart
            .FirstOrDefaultAsync(c => c.CustomerId == message.CustomerId);

        if (shoppingCart != null)
        {
            context.CustomerShoppingCart.Remove(shoppingCart);
            await context.SaveChangesAsync();
        }
    }
}