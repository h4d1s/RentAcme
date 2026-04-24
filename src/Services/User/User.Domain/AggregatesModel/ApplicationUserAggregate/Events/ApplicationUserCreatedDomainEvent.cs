using MediatR;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate.Events;

public class ApplicationUserCreatedDomainEvent : INotification
{
    public ApplicationUser User { get; private set; }

    public ApplicationUserCreatedDomainEvent(ApplicationUser user)
    {
        User = user;
    }
}