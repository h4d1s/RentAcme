using EventBus.Commands;
using EventBus.Constants;
using EventBus.Events;
using EventBus.Events.Interfaces;
using MassTransit;
using SagaOrchestrationStateMachine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOrchestrationStateMachine.StateMachines;

public class BookingStateMachine 
    : MassTransitStateMachine<BookingState>
{
    private readonly ILogger _logger;

    // Events
    public Event<BookingReservedIntegrationEvent> BookingReservedEvent { get; private set; } = null!;
    public Event<PaymentCompletedIntegrationEvent> PaymentCompletedEvent { get; private set; } = null!;
    public Event<PaymentFailedIntegrationEvent> PaymentFailedEvent { get; private set; } = null!;

    // States
    public State Reserved { get; private set; } = null!;
    public State PaymentCompleted { get; private set; } = null!;
    public State PaymentFailed { get; private set; } = null!;
    public State Error { get; private set; } = null!;

    public BookingStateMachine(ILogger<BookingStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState, Reserved, PaymentCompleted, PaymentFailed, Error);

        Event(() => BookingReservedEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => PaymentCompletedEvent, y => y.CorrelateById(b => b.Message.CorrelationId));
        Event(() => PaymentFailedEvent, y => y.CorrelateById(b => b.Message.CorrelationId));

        Initially(
           When(BookingReservedEvent)
                .Then(context => { _logger.LogInformation($"BookingReservedEvent received in BookingStateMachine: {context.Saga}"); })
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.VehicleId = context.Message.VehicleId;
                    context.Saga.PickupDate = context.Message.PickupDate;
                    context.Saga.ReturnDate = context.Message.ReturnDate;
                    context.Saga.Price = context.Message.Price;
                    context.Saga.CorrelationId = Guid.NewGuid();
                })
                .Publish(
                    context => new CompletePaymentCommand
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        UserId = context.Saga.UserId,
                        TotalPrice = context.Saga.Price,
                    })
                .Then(context => { _logger.LogInformation($"CompletePaymentCommand sent in OrderStateMachine: {context.Saga}"); })
                .TransitionTo(Reserved)
                .Catch<Exception>(ex => ex
                    .TransitionTo(Error)
                    .Then(context => _logger.LogError($"Error: {context.Exception}"))
                )
        );

        During(Reserved,
            When(PaymentCompletedEvent)
                .Then(context => { _logger.LogInformation($"PaymentCompletedEvent received in BookingStateMachine: {context.Saga}"); })
                .TransitionTo(PaymentCompleted)
                .Publish(
                    context => new BookingCompletedIntegrationEvent
                    {
                        BookingId = context.Saga.BookingId,
                        VehicleId = context.Saga.VehicleId,
                        PickupDate = context.Saga.PickupDate,
                        ReturnDate = context.Saga.ReturnDate,
                    })
                .Then(context => { _logger.LogInformation($"BookingCompletedIntegrationEvent sent in OrderStateMachine: {context.Saga}"); }),
            When(PaymentFailedEvent)
                .Then(context => { _logger.LogInformation($"PaymentFailedEvent received in BookingStateMachine: {context.Saga}"); })
                .TransitionTo(PaymentFailed)
                .Publish(
                    context => new BookingFailedIntegrationEvent
                    {
                        BookingId = context.Saga.BookingId
                    })
                .Then(context => { _logger.LogInformation($"BookingFailedIntegrationEvent published in BookingStateMachine: {context.Saga}"); })
                .Catch<Exception>(ex => ex
                    .TransitionTo(Error)
                    .Then(context => _logger.LogError($"Error: {context.Exception}")))
            );
    }
}
