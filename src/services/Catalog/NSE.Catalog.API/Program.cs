using NSE.Catalog.API.Configuration;
using NSE.WebAPI.Core.Configuration;
using NSE.WebAPI.Core.Identity;

#region Configure Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddQueueConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.RegisterServices();
#endregion

#region Configure Pipeline
var app = builder.Build();

DbMigrationHelpers.EnsureSeedData(app).Wait();
app.UseSwaggerConfiguration();
app.UseApiCoreConfiguration(app.Environment);

app.Run();

namespace NSE.Catalog.API
{
    public partial class Program;
}
#endregion