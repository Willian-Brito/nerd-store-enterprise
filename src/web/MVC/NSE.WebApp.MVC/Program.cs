using NetDevPack.OpenTelemetry.Otlp;
using NSE.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDevPackTracingOtlp(builder.Environment.ApplicationName);
builder.Services.AddAuthConfiguration();
builder.Services.AddMvcConfiguration(builder.Configuration);
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.UseMvcConfiguration(app.Environment);
await app.RunAsync();
