using MassTransit;

namespace SagaOrchestrationStateMachine.States;

public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }

    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public DateTime? BookingDate { get; set; }
    public decimal Price { get; set; }
}
