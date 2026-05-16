using NSE.Core.Messages.Integration;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Configuration;
using NSE.WebAPI.Core.Extensions;
using NSE.WebAPI.Core.Http;

namespace NSE.Identity.API.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Kafka Implementation
        var baseAddress = configuration["CustomerUrl"];
        services.AddSingleton<IRestClient, RestClient>();
        services.AddHttpClient(nameof(UserRegisteredIntegrationEvent), options =>
        {
            options.BaseAddress = new Uri($"{baseAddress}/api/customers/create");
        })
        .AllowSelfSignedCertificate(); 
        
        services.AddControllers();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddDefaultHealthCheck(configuration);

        return services;
    }
    
    public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) 
            app.UseDeveloperExceptionPage();
        
        // Under certain scenarios, e.g minikube / linux environment / behind load balancer
        // https redirection could lead dev's to over complicated configuration for testing purpouses
        // In production is a good practice to keep it true
        if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
            app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthConfiguration();
        app.UseJwksDiscovery();
        app.UseDefaultHealthcheck();

        return app;
    }
}