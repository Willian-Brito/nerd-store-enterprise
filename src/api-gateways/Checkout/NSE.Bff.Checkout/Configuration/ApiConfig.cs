using NSE.Bff.Checkout.Extensions;
using NSE.WebAPI.Core.Configuration;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Bff.Checkout.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<AppServicesSettings>(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy("Total",
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });

        services.AddDefaultHealthCheck(configuration)
            .AddUrlGroup(
                new Uri($"{configuration["ShoppingCartUrl"]}/healthz-infra"),
                "Shopping Cart",
                tags: ["infra"],
                configurePrimaryHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler()
            )
            .AddUrlGroup(
                new Uri($"{configuration["CatalogUrl"]}/healthz-infra"),
                "Catalog API",
                tags: ["infra"],
                configurePrimaryHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler()
            )
            .AddUrlGroup(
                new Uri($"{configuration["CustomerUrl"]}/healthz-infra"),
                "Customer API",
                tags: ["infra"],
                configurePrimaryHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler()
            );
        // .AddUrlGroup(
        //     new Uri($"{configuration["PaymentUrl"]}/healthz-infra"), 
        //     "Payment API",
        //     tags: ["infra"],
        //     configurePrimaryHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler()
        // )
        // .AddUrlGroup(
        //     new Uri($"{configuration["OrderUrl"]}/healthz-infra"), 
        //     "Order API", 
        //     tags: ["infra"],
        //     configurePrimaryHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler()
        // );
    }
    
    public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        // Under certain scenarios, e.g. minikube / linux environment / behind load balancer
        // https redirection could lead dev's to overcomplicated configuration for testing purposes
        // In production is a good practice to keep it true
        if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
            app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("Total");
        app.UseAuthConfiguration();
        app.MapControllers();
        app.UseDefaultHealthcheck();
    }
}