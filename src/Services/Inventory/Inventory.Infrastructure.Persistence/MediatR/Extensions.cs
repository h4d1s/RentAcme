using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.Data;
using MediatR;

namespace Inventory.Infrastructure.Persistence.MediatR;

static class Extensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, InventoryDbContext ctx)
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
