using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace NSE.WebAPI.Core.Extensions;

public static class QueueExtensions
{
    public static IHealthChecksBuilder AddQueueHealthCheck(
        this IHealthChecksBuilder healthChecksBuilder,
        string connectionString
    )
    {
        healthChecksBuilder.AddRabbitMQ(async _ =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    AutomaticRecoveryEnabled = true
                };
                return await connectionFactory.CreateConnectionAsync();
            },
            tags: ["infra"]
        );

        return healthChecksBuilder;
    }
}