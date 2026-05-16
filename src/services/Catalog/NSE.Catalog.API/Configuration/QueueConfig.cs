using NSE.Catalog.API.Jobs;
using NSE.MessageBroker.Configuration;

namespace NSE.Catalog.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration).AddHostedService<CatalogIntegrationJob>();
    }
}