using NSE.MessageBroker.Configuration;
using NSE.ShoppingCart.API.Jobs;

namespace NSE.ShoppingCart.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration).AddHostedService<ShoppingCartIntegrationJob>();
    }
}