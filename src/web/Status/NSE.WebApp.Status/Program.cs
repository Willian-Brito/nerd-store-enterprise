using Microsoft.EntityFrameworkCore.Diagnostics;
using NSE.WebAPI.Core.DatabaseFlavor;
using NSE.WebAPI.Core.Extensions;

#region Configure Services
var builder = WebApplication.CreateBuilder(args);

var healthCheckBuilder = builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetHeaderText("NerdStore - Status Page");
    var endpoints = builder.Configuration.GetSection("ENDPOINTS").Get<string>();

    foreach (var endpoint in endpoints.Split(";"))
    {
        var name = endpoint.Split('|')[0];
        var uri = endpoint.Split('|')[1];

        setup.AddHealthCheckEndpoint(name, uri);
    }

    setup.UseApiEndpointHttpMessageHandler(sp => HttpExtensions.ConfigureClientHandler());
});

var (database, connString) = DatabaseProviderDetector.Detect(builder.Configuration);

switch (database)
{
    case DatabaseType.None:
        healthCheckBuilder.AddInMemoryStorage();
        break;
    case DatabaseType.SqlServer:
        healthCheckBuilder.AddSqlServerStorage(connString);
        break;
    case DatabaseType.MySql:
        healthCheckBuilder.AddMySqlStorage(connString);
        break;
    case DatabaseType.Postgre:
        healthCheckBuilder.AddPostgreSqlStorage(connString, options =>
        {
            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning)
            );
        });
        break;
    case DatabaseType.Sqlite:
        healthCheckBuilder.AddSqliteStorage(connString);
        break;
    default:
        healthCheckBuilder.AddInMemoryStorage();
        break;
}
#endregion

#region Configure App
var app = builder.Build();

// Under certain scenarios, e.g minikube / linux environment / behind load balancer
// https redirection could lead dev's to over complicated configuration for testing purpouses
// In production is a good practice to keep it true
if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.MapHealthChecksUI(setup =>
{
    setup.AddCustomStylesheet("nerdstore.css");
    setup.UIPath = "/";
    setup.PageTitle = "NerdStore - Status";
});
app.Run();
#endregion
