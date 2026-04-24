namespace EventBus.Events;

public class UserCreatedIntegrationEvent
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
