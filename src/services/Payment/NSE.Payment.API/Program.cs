using NSE.Payment.API.Configuration;
using NSE.WebAPI.Core.Configuration;
using NSE.WebAPI.Core.Identity;

#region Configure Services

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddQueueConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.RegisterServices();
#endregion

#region Configura Pipeline
var app = builder.Build();

await DbMigrationHelpers.EnsureSeedData(app);
app.UseApiCoreConfiguration(app.Environment);

app.Run();
#endregion