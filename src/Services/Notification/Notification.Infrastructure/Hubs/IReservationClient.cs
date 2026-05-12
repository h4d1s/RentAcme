namespace Notification.Infrastructure.Hubs;

public interface IReservationClient
{
    Task OnPaymentIntentCreated(object data);
}
