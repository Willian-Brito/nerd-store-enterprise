using System.Reflection;
using NSE.Catalog.API.Services;
using NSE.Queue.RabbitMQ.Configuration;

namespace NSE.Catalog.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetMessageQueueConnection("MessageBus");
        services.AddQueueRabbitMq(connectionString);
            // .AddHostedService<CatalogIntegrationHandler>();
    }
}