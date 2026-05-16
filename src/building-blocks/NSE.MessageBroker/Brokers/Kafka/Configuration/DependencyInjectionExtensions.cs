using Microsoft.Extensions.DependencyInjection;
using NSE.MessageBroker.Abstractions;
using NSE.MessageBroker.Brokers.Kafka.Adapters;

namespace NSE.MessageBroker.Brokers.Kafka.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddQueueKafka(
        this IServiceCollection services, 
        string bootstrapServer
    )
    {
        if (string.IsNullOrEmpty(bootstrapServer)) throw new ArgumentNullException();
        
        services.AddSingleton<IQueue>(new KafkaAdapter(bootstrapServer));
        return services;
    }
}