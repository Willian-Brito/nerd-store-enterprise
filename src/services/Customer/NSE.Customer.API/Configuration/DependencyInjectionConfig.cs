using NSE.Core.Bus;
using NSE.Customer.API.Data;
using NSE.Customer.API.Data.Models.Interfaces;
using NSE.Customer.API.Data.Repository.Customer;
using NSE.Security.Identity.User;

namespace NSE.Customer.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddScoped<IMessageBus, MessageBus>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<CustomerContext>();
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
        );
    }
}