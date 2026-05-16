using NSE.Core.Messages.Integration;
using NSE.Payment.API.Data;
using NSE.Payment.API.Data.Repository;
using NSE.Payment.API.Facade;
using NSE.Payment.API.Models;
using NSE.Payment.API.Services;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Http;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Payment.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Kafka Implementation
        var baseAddress = configuration["PaymentUrl"];
        services.AddSingleton<IRestClient, RestClient>();
        services.AddHttpClient(nameof(OrderInitiatedIntegrationEvent), options =>
        {            
            options.BaseAddress = new Uri($"{baseAddress}/api/payment/order-initiated");
        })
        .AllowSelfSignedCertificate();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentFacade, CreditCardPaymentFacade>();

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<PaymentContext>();
    }
}