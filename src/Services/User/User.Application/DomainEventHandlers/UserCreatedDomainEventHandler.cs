using EventBus.Events;
using EventBus.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.ApplicationUserAggregate.Events;

namespace User.Application.DomainEventHandlers;

public class UserCreatedDomainEventHandler : INotificationHandler<ApplicationUserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;
    private readonly IIntegrationEventService _integrationService;

    public UserCreatedDomainEventHandler(
        ILogger<UserCreatedDomainEventHandler> logger,
        IIntegrationEventService integrationService)
    {
        _logger = logger;
        _integrationService = integrationService;
    }

    public async Task Handle(ApplicationUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User with Id: {UserId} has been successfully created.", notification.User.Id);

        var userCreatedEvent = new UserCreatedIntegrationEvent
        {
            Id = notification.User.Id,
            ExternalId = notification.User.ExternalId,
            Email = notification.User.Email ?? string.Empty,
            FirstName = notification.User.FirstName,
            LastName = notification.User.LastName,
        };
        await _integrationService.PublishAsync(userCreatedEvent);
    }
}
