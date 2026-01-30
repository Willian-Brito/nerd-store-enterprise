namespace NSE.Catalog.API.Services;

public class CatalogIntegrationHandler : BackgroundService
{
    // private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public CatalogIntegrationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
    
    // public async Task Consume(ConsumeContext<OrderAuthorizedIntegrationEvent> context)
    // {
    //     await WriteDownInventory(context.Message);
    // }
    
    // private async Task WriteDownInventory(OrderAuthorizedIntegrationEvent message)
    // {
    //     using var scope = _serviceProvider.CreateScope();
    //
    //     var productsWithAvailableStock = new List<Product>();
    //     var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    //
    //     var productsId = string.Join(",", message.Items.Select(c => c.Key));
    //     var products = await productRepository.GetProductsById(productsId);
    //
    //     if (products.Count != message.Items.Count)
    //     {
    //         await CancelOrderWithoutStock(message);
    //         return;
    //     }
    //
    //     foreach (var product in products)
    //     {
    //         var productUnits = message.Items.FirstOrDefault(p => p.Key == product.Id).Value;
    //
    //         if (!product.IsAvailable(productUnits))
    //             continue;
    //
    //         product.TakeFromInventory(productUnits);
    //         productsWithAvailableStock.Add(product);
    //     }
    //
    //     if (productsWithAvailableStock.Count != message.Items.Count)
    //     {
    //         await CancelOrderWithoutStock(message);
    //         return;
    //     }
    //
    //     foreach (var product in productsWithAvailableStock) productRepository.Update(product);
    //
    //     if (!await productRepository.UnitOfWork.Commit())
    //         throw new DomainException($"Problems updating stock for order {message.OrderId}");
    //
    //     var productTaken = new OrderLoweredStockIntegrationEvent(message.CustomerId, message.OrderId);
    //     await _bus.Publish(productTaken);
    // }
    //
    // private async Task CancelOrderWithoutStock(OrderAuthorizedIntegrationEvent message)
    // {
    //     var orderCancelled = new OrderCanceledIntegrationEvent(message.CustomerId, message.OrderId);
    //     await _bus.Publish(orderCancelled);
    // }
}