using FluentValidation.Results;
using MediatR;
using NSE.Core.Bus;
using NSE.Order.Application.Commands;
using NSE.Order.Application.Events;
using NSE.Order.Application.Queries.Order;
using NSE.Order.Application.Queries.Voucher;
using NSE.Order.Domain.Interfaces;
using NSE.Order.Infra.Context;
using NSE.Order.Infra.Repository;
using NSE.Security.Identity.User;
using NSE.Core.Messages.Integration;
using NSE.WebAPI.Core.Http;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Order.API.Configuration;

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

        // API
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        // Application
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddScoped<IVoucherQueries, VoucherQueries>();
        services.AddScoped<IOrderQueries, OrderQueries>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        
        // Commands
        services.AddScoped<IRequestHandler<AddOrderCommand, ValidationResult>, OrderCommandHandler>();

        // Events
        services.AddScoped<INotificationHandler<OrderDoneEvent>, OrderEventHandler>();
        
        // Data
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<OrdersContext>();
    }
}