using NSE.Core.DomainObjects;

namespace NSE.Order.Domain.Entities.Orders;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; }
    public decimal Price { get; }
    public string ProductImage { get; set; }

    // EF Rel.
    public Order Order { get; set; }
    
    // EF Constructor
    protected OrderItem() { }
    
    public OrderItem(
        Guid productId, 
        string productName, 
        int quantity,
        decimal price, 
        string productImage = null
    )
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
        ProductImage = productImage;
    }
    
    internal decimal CalculateAmount()
    {
        return Quantity * Price;
    }
}