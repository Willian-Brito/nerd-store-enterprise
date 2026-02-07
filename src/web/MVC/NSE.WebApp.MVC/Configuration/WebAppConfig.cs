using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using NSE.WebAPI.Core.Configuration;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Extensions.Middleware;

namespace NSE.WebApp.MVC.Configuration;

public static class WebAppConfig
{
    public static void AddMvcConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"/var/data_protection_keys/"))
            .SetApplicationName("NerdStoreEnterprise");

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.Configure<AppSettings>(configuration);
        services.AddDefaultHealthCheck(configuration);
    }
    
    public static void UseMvcConfiguration(this WebApplication app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();

        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // app.UseExceptionHandler("/error/500");
            // app.UseStatusCodePagesWithRedirects("/error/{0}");
        }
        else
        {
            app.UseExceptionHandler("/error/500");
            app.UseStatusCodePagesWithRedirects("/error/{0}");
        }

        // Under certain scenarios, e.g minikube / linux environment / behind load balancer
        // https redirection could lead dev's to over complicated configuration for testing purpouses
        // In production is a good practice to keep it true
        if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthConfiguration();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseDefaultHealthcheck();
        app.MapControllerRoute(
            "default",
            "{controller=Catalog}/{action=Index}/{id?}"
        );
    }
}