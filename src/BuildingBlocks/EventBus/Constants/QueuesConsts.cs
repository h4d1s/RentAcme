namespace EventBus.Constants;

public class QueuesConsts
{
    // Commands
    public const string VehicleLockCommandQueueName = "vehicle-lock-command-queue";
    public const string UnlockVehicleCommandQueueName = "unlock-vehicle-command-queue";
    public const string CreatePaymentIntentCommandQueueName = "create-payment-intent-command-queue";

    // Events
    public const string BookingReservedEventQueueName = "booking-reserved-queue";
    public const string BookingCanceledEventQueueName = "booking-canceled-queue";
    public const string BookingCompletedEventQueueName = "booking-completed-queue";
    public const string BookingFailedEventQueueName = "booking-failed-queue";

    public const string PaymentCompletedEventQueueName = "payment-completed-queue";
    public const string PaymentFailedEventQueueName = "payment-failed-queue";

    public const string UserCreatedEventQueueName = "user-created-queue";
}
