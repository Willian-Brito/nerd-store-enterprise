using NSE.MessageBroker.Configuration;

namespace NSE.Identity.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration);
    }
}