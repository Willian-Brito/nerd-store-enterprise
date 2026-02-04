namespace NSE.Core.Messages.Integration;

public class OrderLoweredStockIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    
    public OrderLoweredStockIntegrationEvent(Guid customerId, Guid orderId)
    {
        CustomerId = customerId;
        OrderId = orderId;
    }
}