
using NSE.Identity.API.Configuration;
using NSE.Identity.API.Data;

#region Configure Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddMessageBusConfiguration(builder.Configuration);
#endregion

#region Configure Pipeline
var app = builder.Build();
DbMigrationHelpers.EnsureSeedData(app).Wait();

app.UseSwaggerConfiguration();
app.UseApiConfiguration(app.Environment);
app.MapControllers();

app.Run();
#endregion