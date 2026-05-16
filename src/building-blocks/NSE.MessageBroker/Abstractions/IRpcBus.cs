using EasyNetQ;
using EasyNetQ.Internals;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;

namespace NSE.MessageBroker.Abstractions;

public interface IRpcBus
{
    bool IsConnected { get; }
    IAdvancedBus AdvancedBus { get; }
    
    // Rpc (Request/Response)
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;

    AwaitableDisposable<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
        where TRequest : IntegrationEvent
        where TResponse : ResponseMessage;
}