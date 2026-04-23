using Reservation.API;
using Reservation.Application;
using Reservation.Infrastructure;
using Reservation.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddInfrastructureServices(builder.Configuration, builder.Host)
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApiServices(builder.Configuration, builder.Host);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app
    .ConfigureInfrastructureServices(app.Environment, app.Services, app.Configuration, app.Lifetime)
    .ConfigureInfrastructurePersistenceServices(app.Environment, app.Services)
    .ConfigureApiServices(app.Environment, app.Services);

app.MapControllers();

app.Run();
