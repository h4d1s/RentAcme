using Inventory.API;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Host)
    .AddApplicationServices(builder.Environment, builder.Configuration)
    .AddApiServices(builder.Configuration, builder.Host);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app
    .ConfigureInfrastructureServices(app.Environment, app.Services, app.Configuration, app.Lifetime)
    .ConfigureInfrastructurePersistenceServices(app.Environment, app.Services)
    .ConfigureApiServices(app.Environment, app.Services);

app.ConfigureEndpointApiServices();

app.MapControllers();

app.Run();
