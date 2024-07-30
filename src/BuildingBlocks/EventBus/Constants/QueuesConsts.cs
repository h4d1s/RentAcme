using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Constants;

public class QueuesConsts
{
    // Commands
    public const string CompletePaymentCommandQueueName = "complete-payment-command-queue";

    // Events
    public const string BookingReservedEventQueueName = "booking-reserved-queue";
    public const string BookingCanceledEventQueueName = "booking-canceled-queue";
    public const string BookingCompletedEventQueueName = "booking-completed-queue";
    public const string BookingFailedEventQueueName = "booking-failed-queue";

    public const string PaymentCompletedEventQueueName = "payment-completed-queue";
    public const string PaymentFailedEventQueueName = "payment-failed-queue";

    public const string UserCreatedEventQueueName = "user-created-queue";
}
