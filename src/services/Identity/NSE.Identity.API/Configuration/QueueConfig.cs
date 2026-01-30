using System.Reflection;
using NSE.Queue.RabbitMQ.Configuration;

namespace NSE.Identity.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddQueueRabbitMq(configuration.GetMessageQueueConnection("MessageBus"));
    }
}