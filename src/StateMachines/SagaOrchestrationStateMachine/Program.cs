using MassTransit;
using Microsoft.EntityFrameworkCore;
using Persistence;
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
        });

    var configuration = builder.Configuration;
    cfg.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host(
            configuration["RabbitMQ:Hostname"],
            "/",
            hostConfigurator =>
            {
                hostConfigurator.Username(configuration["RabbitMQ:Username"] ?? throw new ArgumentNullException("RabbitMQ:Username is not configured"));
                hostConfigurator.Password(configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("RabbitMQ:Password is not configured"));
            });
        busFactoryConfigurator.ConfigureEndpoints(context);
    });

    cfg.AddInMemoryInboxOutbox();
});

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
