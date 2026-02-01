using NSE.Core.Messages.Base;

namespace NSE.Order.Application.Events;

public class OrderDoneEvent : Event
{
    public OrderDoneEvent(Guid orderId, Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }

    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
}