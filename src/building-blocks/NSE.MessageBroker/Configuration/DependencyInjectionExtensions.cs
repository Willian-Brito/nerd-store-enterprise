using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.MessageBroker.Brokers.Kafka.Configuration;
using NSE.MessageBroker.Brokers.RabbitMQ.Configuration;

namespace NSE.MessageBroker.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration
            .GetSection("MessageBroker")
            .Get<QueueSettings>();

        switch (settings.Provider)
        {
            case MessageBroker.RabbitMq:
                services.AddQueueRabbitMq(settings.ConnectionString);
                break;

            case MessageBroker.Kafka:
                var bootstrapServer = settings.ConnectionString;
                services.AddQueueKafka(bootstrapServer);
                break;

            default:
                throw new NotImplementedException();
        }

        return services;
    }
}