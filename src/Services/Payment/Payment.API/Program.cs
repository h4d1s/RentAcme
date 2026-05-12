using Payment.API;
using Payment.Application;
using Payment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddApiServices(builder.Configuration, builder.Host)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Host);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app
    .ConfigureApiServices(app.Environment, app.Services)
    .ConfigureInfrastructureServices(app.Environment, app.Services);

app.MapControllers();

app.Run();
