using NSE.MessageBroker.Configuration;
using NSE.Order.Infra.Jobs;

namespace NSE.Order.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration)
            .AddHostedService<OrderOrchestratorIntegrationJob>()
            .AddHostedService<OrderIntegrationJob>();
    }
}