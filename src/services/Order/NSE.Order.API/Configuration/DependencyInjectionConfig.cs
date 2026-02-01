using NSE.Core.Bus;
using NSE.Order.Application.Queries.Order;
using NSE.Order.Application.Queries.Voucher;
using NSE.Order.Domain.Interfaces;
using NSE.Order.Infra.Context;
using NSE.Order.Infra.Repository;
using NSE.Security.Identity.User;

namespace NSE.Order.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        // API
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        // Application
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddScoped<IVoucherQueries, VoucherQueries>();
        services.AddScoped<IOrderQueries, OrderQueries>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        // Date
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<OrdersContext>();
    }
}