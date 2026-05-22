using NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.UnitTests.Factories;

public static class OrderFactory
{
    public static Order.Domain.Entities.Orders.Order Create()
    {
        var items = new List<OrderItem>();
        var item = new OrderItem(
            productId: Guid.NewGuid(),
            productName: "Mouse",
            quantity: 2,
            price: 100M
        );
        
        items.Add(item);
        
        return new Order.Domain.Entities.Orders.Order(
            Guid.NewGuid(),
            200,
            items
        );
    }
}