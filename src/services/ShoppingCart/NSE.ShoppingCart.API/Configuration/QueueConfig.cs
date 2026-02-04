using NSE.Queue.RabbitMQ.Configuration;
using NSE.ShoppingCart.API.Jobs;

namespace NSE.ShoppingCart.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetMessageQueueConnection("MessageBus");
        services.AddQueueRabbitMq(connectionString)
            .AddHostedService<ShoppingCartIntegrationJob>();
    }
}