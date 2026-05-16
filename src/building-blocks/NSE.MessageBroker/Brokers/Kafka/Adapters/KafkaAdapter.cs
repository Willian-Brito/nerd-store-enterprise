using Confluent.Kafka;
using EasyNetQ;
using EasyNetQ.Internals;
using NetDevPack.OpenTelemetry.Extensions;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.MessageBroker.Abstractions;
using NSE.MessageBroker.Brokers.Kafka.Serialization;
using NSE.MessageBroker.Brokers.Kafka.Configuration;

namespace NSE.MessageBroker.Brokers.Kafka.Adapters;

public class KafkaAdapter : IQueue
{
    private readonly string _bootstrapServer;

    public KafkaAdapter(string bootstrapServer)
    {
        _bootstrapServer = bootstrapServer;
    }

    public async Task PublishAsync<T>(T message, string topic = null) where T : IntegrationEvent
    {
        await ProducerAsync(message, topic);
    }

    public async Task SubscribeAsync<T>(
        string topic, 
        Func<T, Task> onMessage, 
        CancellationToken cancellationToken = default
    ) where T : IntegrationEvent
    {
        await ConsumerAsync(onMessage, cancellationToken, topic);
    }
    
    private async Task ProducerAsync<T>(T message, string topic = null) where T : IntegrationEvent
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _bootstrapServer
        };

        var headers = new Dictionary<string, string>();
        headers["transactionId"] = Guid.NewGuid().ToString();
        var activity = NetDevPackExtensions.StartProducer(headers, $"Producer {topic}");
        
        var producer = new ProducerBuilder<string, T>(config)
            .SetValueSerializer(new KafkaSerializer<T>())
            .Build();
        
        var result = await producer.ProduceAsync(topic, new Message<string, T>
        {
            Key = Guid.NewGuid().ToString(),
            Value = message, 
            Headers = headers.DictionaryToHeader()
        });

        await Task.CompletedTask;
    }

    private async Task ConsumerAsync<T>(
        Func<T, Task> onMessage, 
        CancellationToken cancellation,
        string topic = null
    ) where T : IntegrationEvent
    {
        _ = Task.Factory.StartNew(async () =>
        {
            var config = new ConsumerConfig
            {
                GroupId = "nerdstore-enterprise",
                BootstrapServers = _bootstrapServer,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
            };
            
            using var consumer = new ConsumerBuilder<string, T>(config)
                .SetValueDeserializer(new KafkaDeserializer<T>())
                .Build();
            
            consumer.Subscribe(topic);

            while (!cancellation.IsCancellationRequested)
            {
                var result = consumer.Consume();

                if (result.IsPartitionEOF)
                    continue;

                var headers = result.Message.Headers.HeaderToDictionary();
                NetDevPackExtensions.StartConsumer(headers, $"Consumer: {topic}");
                
                await onMessage(result.Message.Value);
                consumer.Commit();
            }
        }, cancellation, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        await Task.CompletedTask;
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}