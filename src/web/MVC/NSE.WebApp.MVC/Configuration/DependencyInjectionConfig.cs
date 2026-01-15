using Microsoft.AspNetCore.Mvc.DataAnnotations;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Extensions;
using NSE.WebApp.MVC.Extensions.Annotations;
using NSE.WebApp.MVC.Services.Auth;
using Polly;

namespace NSE.WebApp.MVC.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IValidationAttributeAdapterProvider, SocialNumberValidationAttributeAdapterProvider>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        // services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        //
        services
            .AddHttpClient<IAuthService, AuthService>();
            // .AddPolicyHandler(PollyExtensions.WaitAndRetry())
            // .AllowSelfSignedCertificate()
        // .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        //
        // services.AddHttpClient<ICatalogService, CatalogService>()
        //     .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        //     .AddPolicyHandler(PollyExtensions.WaitAndRetry())
        //     .AllowSelfSignedCertificate()
        //     .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
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

// public static class PollyExtensions
// {
//     public static AsyncRetryPolicy<HttpResponseMessage> WaitAndRetry()
//     {
//         var retry = HttpPolicyExtensions
//             .HandleTransientHttpError()
//             .WaitAndRetryAsync(new[]
//             {
//                 TimeSpan.FromSeconds(1),
//                 TimeSpan.FromSeconds(5),
//                 TimeSpan.FromSeconds(10)
//             }, (outcome, timespan, retryCount, context) =>
//             {
//                 Console.ForegroundColor = ConsoleColor.Blue;
//                 Console.WriteLine($"Tentando pela {retryCount} vez!");
//                 Console.ForegroundColor = ConsoleColor.White;
//             });
//
//         return retry;
//     }
// }