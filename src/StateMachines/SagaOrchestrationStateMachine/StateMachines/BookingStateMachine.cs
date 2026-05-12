using EventBus.Commands;
using EventBus.Events;
using MassTransit;
using SagaOrchestrationStateMachine.States;

namespace SagaOrchestrationStateMachine.StateMachines;

public class BookingStateMachine
    : MassTransitStateMachine<BookingState>
{
    private readonly ILogger _logger;

    // Events
    public Event<BookingReservedIntegrationEvent> BookingReservationEvent { get; private set; } = null!;

    public Event<VehicleLockedIntegrationEvent> VehicleLockedEvent { get; private set; } = null!;
    public Event<VehicleUnlockedIntegrationEvent> VehicleUnlockedEvent { get; private set; } = null!;
    public Event<VehicleUnavailableIntegrationEvent> VehicleUnavailableEvent { get; private set; } = null!;

    public Event<PaymentCompletedIntegrationEvent> PaymentCompletedEvent { get; private set; } = null!;
    public Event<PaymentFailedIntegrationEvent> PaymentFailedEvent { get; private set; } = null!;

    // States
    public State VehicleLockPending { get; private set; } = null!;
    public State VehicleReleasePending { get; private set; } = null!;

    public State PaymentPending { get; private set; } = null!;

    public State Failed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    // Timeouts
    public Schedule<BookingState, VehicleLockTimeoutExpiredIntegrationEvent> VehicleLockTimeout { get; private set; } = null!;
    public Schedule<BookingState, PaymentTimeoutExpiredIntegrationEvent> PaymentTimeout { get; private set; } = null!;
    public Schedule<BookingState, VehicleReleaseTimeoutExpiredIntegrationEvent> VehicleReleaseTimeout { get; private set; } = null!;

    public BookingStateMachine(ILogger<BookingStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);
        Event(() => BookingReservationEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => VehicleLockedEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => VehicleUnlockedEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => VehicleUnavailableEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => PaymentCompletedEvent, y => y.CorrelateById(b => b.Message.BookingId));
        Event(() => PaymentFailedEvent, y => y.CorrelateById(b => b.Message.BookingId));

        Schedule(() => VehicleLockTimeout, x => x.VehicleLockTimeoutTokenId, s => {
            s.Delay = TimeSpan.FromMinutes(5);
            s.Received = r => r.CorrelateById(m => m.Message.BookingId);
        });
        Schedule(() => PaymentTimeout, x => x.PaymentTimeoutTokenId, s => {
            s.Delay = TimeSpan.FromMinutes(5);
            s.Received = r => r.CorrelateById(m => m.Message.BookingId);
        });
        Schedule(() => VehicleReleaseTimeout, x => x.VehicleReleaseTimeoutTokenId, s => {
            s.Delay = TimeSpan.FromMinutes(2);
            s.Received = r => r.CorrelateById(m => m.Message.BookingId);
        });

        Initially(
           When(BookingReservationEvent)
            .Then(context => { _logger.LogInformation($"BookingReservationEvent received in BookingStateMachine: {context.Saga}"); })
            .Then(context =>
            {
                context.Saga.CorrelationId = context.Message.BookingId;
                context.Saga.UserId = context.Message.UserId;
                context.Saga.VehicleId = context.Message.VehicleId;
                context.Saga.PickupDate = context.Message.PickupDate;
                context.Saga.ReturnDate = context.Message.ReturnDate;
                context.Saga.Price = context.Message.Price;
                context.Saga.BookingDate = DateTime.UtcNow;
            })
            .Schedule(VehicleLockTimeout, context => new VehicleLockTimeoutExpiredIntegrationEvent
            {
                BookingId = context.Saga.CorrelationId
            })
            .Send(context => new LockVehicleCommand
            {
                BookingId = context.Saga.CorrelationId,
                VehicleId = context.Message.VehicleId,
            })
            .TransitionTo(VehicleLockPending)
        );

        During(VehicleLockPending,
           When(VehicleLockTimeout.Received)
                .Then(context => { _logger.LogInformation($"VehicleLockTimeout received in BookingStateMachine: {context.Saga}"); })
                .Then(context =>
                {
                    context.Saga.ErrorMessage = "Vehicle lock timed out";
                })
                .Publish(context => new BookingFailedIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId
                })
                .TransitionTo(Failed),
            When(VehicleLockedEvent)
                .Then(context => { _logger.LogInformation($"VehicleLockedEvent received in BookingStateMachine: {context.Saga}"); })
                .Unschedule(VehicleLockTimeout)
                .Schedule(PaymentTimeout, context => new PaymentTimeoutExpiredIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId
                })
                .Send(context => new CreatePaymentIntentCommand
                {
                    CorrelationId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId,
                    TotalPrice = context.Saga.Price,
                    PaymentMethodId = context.Saga.PaymentMethodId
                })
                .Then(context => { _logger.LogInformation($"CreatePaymentIntentCommand sent in OrderStateMachine: {context.Saga}"); })
                .TransitionTo(PaymentPending),
            When(VehicleUnavailableEvent)
                .Then(context => { _logger.LogInformation($"VehicleUnavailableEvent received in BookingStateMachine: {context.Saga}"); })
                .Unschedule(VehicleLockTimeout)
                .Then(context => context.Saga.ErrorMessage = "Vehicle no longer available.")
                .Publish(context => new BookingFailedIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId
                })
                .TransitionTo(Failed)
        );

        During(PaymentPending,
            When(PaymentTimeout.Received)
                .Then(context => { _logger.LogInformation($"PaymentTimeout received in BookingStateMachine: {context.Saga}"); })
                .Then(context =>
                {
                    context.Saga.ErrorMessage = "Payment lock timed out";
                })
                .Send(context => new UnlockVehicleCommand
                {
                    BookingId = context.Saga.CorrelationId,
                    VehicleId = context.Saga.VehicleId
                })
                .Schedule(VehicleReleaseTimeout, context => new VehicleReleaseTimeoutExpiredIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId
                })
                .TransitionTo(VehicleReleasePending),
            When(PaymentCompletedEvent)
                .Unschedule(PaymentTimeout)
                .Then(context => { _logger.LogInformation($"PaymentCompletedEvent received in BookingStateMachine: {context.Saga}"); })
                .Publish(context => new BookingCompletedIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId,
                    VehicleId = context.Saga.VehicleId,
                    PickupDate = context.Saga.PickupDate,
                    ReturnDate = context.Saga.ReturnDate,
                    Price = context.Saga.Price
                })
                .TransitionTo(Completed),
            When(PaymentFailedEvent)
                .Then(context => { _logger.LogInformation($"PaymentFailedEvent received in BookingStateMachine: {context.Saga}"); })
                .Then(context => context.Saga.ErrorMessage = "Payment failed.")
                .Send(context => new UnlockVehicleCommand
                    {
                        BookingId = context.Saga.CorrelationId,
                        VehicleId = context.Saga.VehicleId
                    })
                .Schedule(VehicleReleaseTimeout, context => new VehicleReleaseTimeoutExpiredIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId
                })
                .TransitionTo(VehicleReleasePending)
        );

        During(VehicleReleasePending,
            When(VehicleReleaseTimeout.Received)
                .Then(context => { _logger.LogInformation($"VehicleReleaseTimeout received in BookingStateMachine: {context.Saga}"); })
                .Then(context =>
                {
                    context.Saga.ErrorMessage = "Vehicle release lock timed out";
                })
                .Publish(context => new BookingFailedIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId
                })
                .TransitionTo(Failed),
            When(VehicleUnlockedEvent)
                .Then(context => { _logger.LogInformation($"VehicleUnlockedEvent received in BookingStateMachine: {context.Saga}"); })
                .Unschedule(VehicleReleaseTimeout)
                .Publish(context => new BookingFailedIntegrationEvent
                {
                    BookingId = context.Saga.CorrelationId,
                    UserId = context.Saga.UserId
                })
                .TransitionTo(Failed)
        );
    }
}
