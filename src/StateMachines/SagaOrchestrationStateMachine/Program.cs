using EventBus.Commands;
using EventBus.Constants;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Quartz;
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
        busFactoryConfigurator.Host(
            configuration["RabbitMQ:Hostname"],
            "/",
            hostConfigurator =>
            {
                hostConfigurator.Username(configuration["RabbitMQ:Username"] ?? throw new ArgumentNullException("RabbitMQ:Username is not configured"));
                hostConfigurator.Password(configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("RabbitMQ:Password is not configured"));
                hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(2));
            });
        busFactoryConfigurator.ConfigureEndpoints(context);
    });
});
builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService();

builder.Services.AddDbContext<SagaMachineContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("SagaMachineContext") ??
            throw new InvalidOperationException("Connection string 'SagaMachineContext' not found.")
        );
});

builder.Services.AddMigration<SagaMachineContext>();

var host = builder.Build();
host.Run();
