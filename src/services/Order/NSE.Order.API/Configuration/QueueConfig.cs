using NSE.Order.Infra.Jobs;
using NSE.Queue.RabbitMQ.Configuration;

namespace NSE.Order.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetMessageQueueConnection("MessageBus");
        services.AddQueueRabbitMq(connectionString)
            .AddHostedService<OrderOrchestratorIntegrationJob>()
            .AddHostedService<OrderIntegrationJob>();
    }
}