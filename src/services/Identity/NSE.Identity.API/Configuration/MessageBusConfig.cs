using System.Reflection;
using NSE.MessageBus.RabbitMQ;

namespace NSE.Identity.API.Configuration;

public static class MessageBusConfig
{
    public static void AddMessageBusConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddRabbitMqMessageBus(configuration, Assembly.GetAssembly(typeof(Program)));
    }
}