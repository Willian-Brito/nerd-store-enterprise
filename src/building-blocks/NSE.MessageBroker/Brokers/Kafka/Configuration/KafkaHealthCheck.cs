using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBroker.Brokers.RabbitMQ.Configuration;

public static class KafkaHealthCheck
{
    public static IHealthChecksBuilder AddKafkaHealthCheck(
        this IHealthChecksBuilder healthChecksBuilder, 
        string bootstrapServers
    )
    {
        return healthChecksBuilder.AddKafka(
            setup =>
            {
                setup.BootstrapServers = bootstrapServers;
            },
            name: "Kafka",
            tags: new[] { "infra" }
        );
    }
}