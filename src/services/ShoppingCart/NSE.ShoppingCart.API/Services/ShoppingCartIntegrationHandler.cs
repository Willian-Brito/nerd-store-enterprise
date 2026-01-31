using Microsoft.EntityFrameworkCore;
using NSE.Core.Messages.Integration;
using NSE.Queue.Abstractions;
using NSE.ShoppingCart.API.Data;

namespace NSE.ShoppingCart.API.Services;

public class ShoppingCartIntegrationHandler : BackgroundService
{
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public ShoppingCartIntegrationHandler(IServiceProvider serviceProvider, IQueue queue)
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
        _queue.SubscribeAsync<OrderDoneIntegrationEvent>(
            "PedidoRealizado", 
            async request => await RemoveShoppingCart(request)
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