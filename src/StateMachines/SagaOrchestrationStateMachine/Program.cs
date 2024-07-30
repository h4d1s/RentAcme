using EventBus.Constants;
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
            hostConfigurator => {
                hostConfigurator.Username(configuration["RabbitMQ:Username"]);
                hostConfigurator.Password(configuration["RabbitMQ:Password"]);
            });
        busFactoryConfigurator.ConfigureEndpoints(context);
    });

    cfg.AddInMemoryInboxOutbox();
});

builder.Services.AddDbContext<SagaMachineContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SagaMachineContext") ??
            throw new InvalidOperationException("Connection string 'SagaMachineContext' not found.")
        );
});

builder.Services.AddMigration<SagaMachineContext>();

var host = builder.Build();
host.Run();
