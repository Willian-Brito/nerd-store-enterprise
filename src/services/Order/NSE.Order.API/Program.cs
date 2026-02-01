using NSE.Order.API.Configuration;
using NSE.WebAPI.Core.Configuration;
using NSE.WebAPI.Core.Identity;

#region Configure Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.RegisterServices();
builder.Services.AddQueueConfiguration(builder.Configuration);
#endregion

#region Configure Pipeline
var app = builder.Build();

DbMigrationHelpers.EnsureSeedData(app).Wait();
app.UseSwaggerConfiguration();
app.UseApiCoreConfiguration(app.Environment);

app.Run();
#endregion
