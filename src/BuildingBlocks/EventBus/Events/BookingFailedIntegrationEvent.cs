namespace EventBus.Events;

public class BookingFailedIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
}
