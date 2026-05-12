using MassTransit;

namespace SagaOrchestrationStateMachine.States;

public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; } // BookingId
    public string CurrentState { get; set; } = string.Empty;

    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public DateTime? BookingDate { get; set; }
    public decimal Price { get; set; }
    public string PaymentMethodId { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; } = string.Empty;

    public Guid? VehicleLockTimeoutTokenId { get; set; }
    public Guid? PaymentTimeoutTokenId { get; set; }
    public Guid? VehicleReleaseTimeoutTokenId { get; set; }
}
