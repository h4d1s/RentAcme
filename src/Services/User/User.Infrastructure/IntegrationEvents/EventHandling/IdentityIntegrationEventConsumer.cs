using EventBus.Events;
using Identity.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Infrastructure.IntegrationEvents.EventHandling;

public class IdentityIntegrationEventConsumer : IConsumer<KeycloakEventIntegrationEvent>
{
    private readonly ILogger<IdentityIntegrationEventConsumer> _logger;
    private IApplicationUserRepository _userRepository;
    private IIdentityManagerService _identityManagementService;

    public IdentityIntegrationEventConsumer(
        IApplicationUserRepository userRepository,
        IIdentityManagerService identityManagementService,
        ILogger<IdentityIntegrationEventConsumer> logger)
    {
        _userRepository = userRepository;
        _identityManagementService = identityManagementService;
        _logger = logger;
    }   

    public async Task Consume(ConsumeContext<KeycloakEventIntegrationEvent> context)
    {
        if (
            context.Message.Type != "REGISTER" ||
            context.Message.Type != "DELETE_USER"
        )
        {
            return;
        }

        var keycloakUserId = context.Message.UserId;
        if (string.IsNullOrEmpty(keycloakUserId))
        {
            return;
        }
        var keycloakUser = await _identityManagementService.GetUserByIdAsync(keycloakUserId);

        if (keycloakUser == null)
        {
            return;
        }

        switch (context.Message.Type)
        {
            case "REGISTER":
                try
                {
                    var newUser = new ApplicationUser(
                        keycloakUserId,
                        keycloakUser.email ?? "",
                        keycloakUser.username ?? "",
                        keycloakUser.firstName ?? "",
                        keycloakUser.lastName ?? "",
                        "+1234657890"
                    );
                    await _userRepository.AddAsync(newUser);
                    await _userRepository.UnitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "KeycloakEventIntegrationEvent");
                    throw;
                }
                break;
            case "DELETE_USER":
                try
                {
                    var userToDelete = await _userRepository.GetByExternalIdAsync(keycloakUserId);
                    if (userToDelete is null)
                    {
                        throw new NotFoundException("User not found");
                    }
                    _userRepository.Delete(userToDelete);
                    await _userRepository.UnitOfWork.SaveChangesAsync();
                    await _identityManagementService.DeleteUserByIdAsync(keycloakUserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "KeycloakEventIntegrationEvent");
                    throw;
                }
                break;
            default:
                break;
        }
    }
}
