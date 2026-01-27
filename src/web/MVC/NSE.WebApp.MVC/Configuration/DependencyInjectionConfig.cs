using Microsoft.AspNetCore.Mvc.DataAnnotations;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Extensions;
using NSE.WebApp.MVC.Extensions.Annotations;
using NSE.WebApp.MVC.Extensions.Retry;
using NSE.WebApp.MVC.Services.Auth;
using NSE.WebApp.MVC.Services.Catalog;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;

namespace NSE.WebApp.MVC.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IValidationAttributeAdapterProvider, SocialNumberValidationAttributeAdapterProvider>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        services
            .AddHttpClient<IAuthService, AuthService>()
            .AddPolicyHandler(PollyExtensions.WaitAndRetry())
            // .AllowSelfSignedCertificate()
        .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        
        services.AddHttpClient<ICatalogService, CatalogService>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddPolicyHandler(PollyExtensions.WaitAndRetry())
        //     .AllowSelfSignedCertificate()
        .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        //
        // services.AddHttpClient<ICheckoutBffService, CheckoutBffService>()
        //     .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        //     .AddPolicyHandler(PollyExtensions.WaitAndRetry())
        //     .AllowSelfSignedCertificate()
        //     .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        //
        // services.AddHttpClient<ICustomerService, CustomerService>()
        //     .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        //     .AddPolicyHandler(PollyExtensions.WaitAndRetry())
        //     .AllowSelfSignedCertificate()
        //     .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
    }
}