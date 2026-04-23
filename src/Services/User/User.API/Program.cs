using User.API;
using User.Application;
using User.Infrastructure;
using User.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Host)
    .AddApplicationServices(builder.Environment)
    .AddApiServices(builder.Configuration, builder.Host);

var app = builder.Build();

app.UseHttpsRedirection();

app
    .ConfigureInfrastructurePersistenceServices(app.Environment, app.Services)
    .ConfigureInfrastructureServices(app.Environment, app.Services, app.Configuration, app.Lifetime)
    .ConfigureApiServices(app.Environment, app.Services);

app.ConfigureEndpointApiServices();

app.MapControllers();

app.Run();
