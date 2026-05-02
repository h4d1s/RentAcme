using Identity.Models;
using Identity.Services;
using MediatR;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Queries.GetUser;

public class GetUserHandler : IRequestHandler<GetUserQuery, ApplicationUser>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IIdentityService _identityService;

    public GetUserHandler(
        IApplicationUserRepository userRepository,
        IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<ApplicationUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User with Id {request.Id} not found.");
        }

        var permissions = _identityService.GetUserPermissions();
        var isOwner = request.Id.ToString() == _identityService.GetUserId();
        var canView = isOwner || permissions.Contains(Permissions.Users.ViewAny);

        if (!canView)
        {
            throw new UnauthorizedAccessException("You are not authorized to view this user.");
        }

        return user;
    }
}
