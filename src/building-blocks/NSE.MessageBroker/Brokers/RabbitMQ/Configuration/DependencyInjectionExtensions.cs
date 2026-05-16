using Microsoft.Extensions.DependencyInjection;
using NSE.MessageBroker.Abstractions;
using NSE.MessageBroker.Brokers.RabbitMQ.Adapters;

namespace NSE.MessageBroker.Brokers.RabbitMQ.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddQueueRabbitMq(
        this IServiceCollection services, 
        string connection
    )
    {
        if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException(nameof(connection));
        
        services.AddSingleton<RabbitMqAdapter>(_ => new RabbitMqAdapter(connection));
        
        // IQueue
        services.AddSingleton<IQueue>(sp => sp.GetRequiredService<RabbitMqAdapter>());

        // IRpcBus
        services.AddSingleton<IRpcBus>(sp => sp.GetRequiredService<RabbitMqAdapter>());
        
        // services.AddSingleton<IQueue>(new RabbitMqAdapter(connection));
        return services;
    }
}