using MediatR;
using NSE.Core.Messages.Integration;
using NSE.Queue.Abstractions;

namespace NSE.Order.Application.Events;

public class OrderEventHandler: INotificationHandler<OrderDoneEvent>
{
    private readonly IQueue _queue;

    public OrderEventHandler(IQueue queue)
    {
        _queue = queue;
    }

    public async Task Handle(OrderDoneEvent message, CancellationToken cancellationToken)
    {
        await _queue.PublishAsync(new OrderDoneIntegrationEvent(message.CustomerId));
    }
}