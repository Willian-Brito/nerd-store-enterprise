
// using NSE.WebAPI.Core.Configuration;
// using NSE.WebAPI.Core.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Security.Identity.User;

namespace NSE.Identity.API.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddScoped<IAspNetUser, AspNetUser>();
        // services.AddDefaultHealthCheck(configuration);

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
        // app.UseJwksDiscovery();
        // app.UseDefaultHealthcheck();

        return app;
    }
}