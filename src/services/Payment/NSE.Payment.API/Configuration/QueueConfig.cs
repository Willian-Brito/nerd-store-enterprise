using NSE.Payment.API.Jobs;
using NSE.MessageBroker.Configuration;

namespace NSE.Payment.API.Configuration;

public static class QueueConfig
{
    public static void AddQueueConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBroker(configuration).AddHostedService<PaymentIntegrationJob>();
    }
}