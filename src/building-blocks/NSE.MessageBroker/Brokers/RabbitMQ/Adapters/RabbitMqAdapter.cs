using EasyNetQ;
using EasyNetQ.Internals;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.MessageBroker.Abstractions;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace NSE.MessageBroker.Brokers.RabbitMQ.Adapters;

public class RabbitMqAdapter : IQueue, IRpcBus
{
    private IBus _bus;
    private readonly string _connectionString;
    private IAdvancedBus _advancedBus;
    public bool IsConnected => _bus?.Advanced.IsConnected ?? false;
    public IAdvancedBus AdvancedBus => _bus?.Advanced;

    public RabbitMqAdapter(string connectionString)
    {
        _connectionString = connectionString;
        TryConnect();
    }

    public async Task PublishAsync<T>(T message, string topic = null) where T : IntegrationEvent
    {  
        TryConnect();
        await _bus.PubSub.PublishAsync(message);
    }

    public async Task SubscribeAsync<T>(
        string topic,
        Func<T, Task> onMessage, 
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent
    {
        TryConnect();
        var subscriptionId = topic;
        await _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {       
        TryConnect();
        return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request);
    }

    public AwaitableDisposable<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {     
        TryConnect();
        return _bus.Rpc.RespondAsync(responder);
    }
    
    private void TryConnect()
    {
        if(IsConnected) return;

        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        policy.Execute(() =>
        {
            _bus = RabbitHutch.CreateBus(_connectionString);
            _advancedBus = _bus.Advanced;
            _advancedBus.Disconnected += OnDisconnect;
        });
    }
    
    private void OnDisconnect(object s, EventArgs e)
    {
        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .RetryForever();

        policy.Execute(TryConnect);
    }

    public void Dispose()
    {
        _bus?.Dispose();
    }
}