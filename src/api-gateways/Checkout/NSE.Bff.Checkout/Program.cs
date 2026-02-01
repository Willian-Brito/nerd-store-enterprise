using NSE.Bff.Checkout.Configuration;
using NSE.WebAPI.Core.Identity;

#region Configure Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.RegisterServices();
builder.Services.ConfigureGrpcServices(builder.Configuration);
#endregion

#region Configure Pipeline
var app = builder.Build();

app.UseSwaggerConfiguration();
app.UseApiConfiguration(app.Environment);

app.Run();
#endregion