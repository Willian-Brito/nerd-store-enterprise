using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Queue.Abstractions;
using NSE.Queue.RabbitMQ.Adapters;

namespace NSE.Queue.RabbitMQ.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddQueueRabbitMq(
        this IServiceCollection services, 
        string connection
    )
    {
        if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException();
        
        services.AddSingleton<IQueue>(new RabbitMqAdapter(connection));
        return services;
    }
}