using MediatR;
using Reservation.Domain.Common;
using Reservation.Infrastructure.Persistence.Data;

namespace Reservation.Infrastructure.Persistence.MediatR;

public static class Extensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, ReservationDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents ?? Enumerable.Empty<INotification>())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
