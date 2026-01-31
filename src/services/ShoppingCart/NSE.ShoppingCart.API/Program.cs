using Microsoft.AspNetCore.Authorization;
using NSE.ShoppingCart.API.Configuration;
using NSE.ShoppingCart.API.Endpoints;
using NSE.ShoppingCart.API.Models;
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

app.UseSwaggerConfiguration();
app.UseApiConfiguration(app.Environment);
DbMigrationHelpers.EnsureSeedData(app).Wait();
#endregion

#region Endpoints

app.MapShoppingCartEndpoints();
app.Run();
#endregion


