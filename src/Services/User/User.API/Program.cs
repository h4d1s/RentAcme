using User.Infrastructure;
using User.Application;
using User.Infrastructure.Persistence;
using Asp.Versioning;
using User.API.Middleware;
using Microsoft.OpenApi.Models;
using User.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Users API",
        Description = "Users microservice",
    });
});

builder.Services
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Host)
    .AddApplicationServices(builder.Environment)
    .AddApiServices(builder.Configuration, builder.Host);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app
    .ConfigureInfrastructurePersistenceServices(app.Environment, app.Services)
    .ConfigureInfrastructureServices(app.Environment, app.Services, app.Configuration, app.Lifetime)
    .ConfigureApiServices(app.Environment, app.Services);

app.ConfigureEndpointApiServices();

app.MapControllers();

app.Run();
