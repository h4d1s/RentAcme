using MediatR;
using User.Domain.Common;
using User.Infrastructure.Persistence.Data;

namespace User.Infrastructure.Persistence.MediatR;

public static class Extensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, ApplicationUserDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
