using EasyNetQ;
using EasyNetQ.Internals;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;

namespace NSE.MessageBroker.Abstractions;

public interface IQueue : IDisposable
{
    // Pub/Sub
    Task PublishAsync<T>(T message, string? topic = null) where T : IntegrationEvent;

    Task SubscribeAsync<T>(
        string topic, 
        Func<T, Task> onMessage, 
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent;
}