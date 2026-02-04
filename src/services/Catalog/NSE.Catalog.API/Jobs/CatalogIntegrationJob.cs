using NSE.Catalog.API.Data.Models.Interfaces;
using NSE.Catalog.API.Models.Entities;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.Queue.Abstractions;

namespace NSE.Catalog.API.Jobs;

public class CatalogIntegrationJob : BackgroundService
{
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public CatalogIntegrationJob(IServiceProvider serviceProvider, IQueue queue)
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
        _queue.SubscribeAsync<OrderAuthorizedIntegrationEvent>(
            "PedidoAutorizado", async request =>
            await WriteDownInventory(request)
        );
    }
    
    private async Task WriteDownInventory(OrderAuthorizedIntegrationEvent message)
    {
        using var scope = _serviceProvider.CreateScope();
    
        var productsWithAvailableStock = new List<Product>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    
        var productsId = string.Join(",", message.Items.Select(c => c.Key));
        var products = await productRepository.GetProductsById(productsId);
    
        if (products.Count != message.Items.Count)
        {
            await CancelOrderWithoutStock(message);
            return;
        }
    
        foreach (var product in products)
        {
            var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;
    
            if (!product.IsAvailable(productUnits))
                continue;
    
            product.TakeFromInventory(productUnits);
            productsWithAvailableStock.Add(product);
        }
    
        if (productsWithAvailableStock.Count != message.Items.Count)
        {
            await CancelOrderWithoutStock(message);
            return;
        }
    
        foreach (var product in productsWithAvailableStock) productRepository.Update(product);
    
        if (!await productRepository.UnitOfWork.Commit())
            throw new DomainException($"Problems updating stock for order {message.OrderId}");
    
        var productTaken = new OrderLoweredStockIntegrationEvent(message.CustomerId, message.OrderId);
        await _queue.PublishAsync(productTaken);
    }
    
    private async Task CancelOrderWithoutStock(OrderAuthorizedIntegrationEvent message)
    {
        var orderCancelled = new OrderCanceledIntegrationEvent(message.CustomerId, message.OrderId);
        await _queue.PublishAsync(orderCancelled);
    }
}