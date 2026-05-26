using Diagnostics;
using EventBus.Commands;
using EventBus.Constants;
using Google.Protobuf.WellKnownTypes;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Persistence;
using Quartz;
using RabbitMQ.Client;
using SagaOrchestrationStateMachine;
using SagaOrchestrationStateMachine.Data;
using SagaOrchestrationStateMachine.StateMachines;
using SagaOrchestrationStateMachine.States;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .EntityFrameworkRepository(c =>
        {
            c.ExistingDbContext<SagaMachineContext>();
            c.UsePostgres();
        });
    cfg.AddInMemoryInboxOutbox();
    cfg.AddQuartzConsumers();

    var configuration = builder.Configuration;
    cfg.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        EndpointConvention.Map<LockVehicleCommand>(new Uri($"queue:{QueuesConsts.VehicleLockCommandQueueName}"));
        EndpointConvention.Map<UnlockVehicleCommand>(new Uri($"queue:{QueuesConsts.UnlockVehicleCommandQueueName}"));
        EndpointConvention.Map<CreatePaymentIntentCommand>(new Uri($"queue:{QueuesConsts.CreatePaymentIntentCommandQueueName}"));

        busFactoryConfigurator.UseMessageScheduler(new Uri("queue:quartz"));
        var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new ArgumentNullException("RabbitMQ connecting string is not configured");

        busFactoryConfigurator.Host(new Uri(connectionString),
            hostConfigurator =>
            {
                hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(2));
            });
        busFactoryConfigurator.ConfigureEndpoints(context);
    });
});
builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService();

builder.Services.AddSingleton<IConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("RabbitMQ")
        ?? throw new ArgumentNullException("RabbitMQ connection string is not configured");
    var factory = new ConnectionFactory
    {
        Uri = new Uri(connectionString),
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddDbContext<SagaMachineContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("SagaMachineContext") ??
            throw new InvalidOperationException("Connection string 'SagaMachineContext' not found.")
        );
});
builder.Services.AddMigration<SagaMachineContext>();

var hcBuilder = builder.Services.AddHealthChecks();
hcBuilder
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddRabbitMQ(sp => sp.GetRequiredService<IConnection>())
    .AddNpgSql(
        builder.Configuration.GetConnectionString("SagaMachineContext") ?? throw new ArgumentNullException("SagaMachineContext is not configured"),
        name: "SagaMachineDB-check",
        tags: new string[] { "saga-machine-db" });
builder.Services.AddHostedService<HealthCheckWebServerInstance>();

// Logging
builder.Services.AddLoggingSerilog("SagaOrchestrationStateMachine", builder, builder.Configuration);

// Diagnostics
builder.Services.AddObservability("SagaOrchestrationStateMachine", builder.Configuration);

var host = builder.Build();
host.Run();
