using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBus.RabbitMQ;

public static class DependencyInjectionExtensions
{
    public static void AddRabbitMqMessageBus(
        this IServiceCollection services, 
        IConfiguration configuration,
        params Assembly[] consumerAssemblies
    )
    {
        services.AddOptions<RabbitMqTransportOptions>()
            .Bind(configuration.GetSection(nameof(RabbitMqTransportOptions)))
            .ValidateOnStart();

        services.AddMassTransit(busRegistration =>
        {
            busRegistration.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "RabbitMQ";
                options.Tags.Add("infra");
            });
            busRegistration.SetRabbitMqReplyToRequestClientFactory();
            busRegistration.AddConsumers(consumerAssemblies);
            busRegistration.UsingRabbitMq((busContext, busConfiguration) =>
            {
                busConfiguration.ConfigureEndpoints(busContext);
            });
        });
    }
}