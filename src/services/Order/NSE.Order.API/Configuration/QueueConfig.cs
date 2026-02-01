using NSE.Order.Application.Events;
using NSE.Queue.RabbitMQ.Configuration;

namespace NSE.Order.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("MessageBus");
        services.AddQueueRabbitMq(connectionString)
            .AddHostedService<OrderOrchestratorIntegrationHandler>()
            .AddHostedService<OrderIntegrationHandler>();
    }
}