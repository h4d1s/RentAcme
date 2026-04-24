using MediatR;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Queries.GetUser;

public class GetUserHandler : IRequestHandler<GetUserQuery, ApplicationUser>
{
    private readonly IApplicationUserRepository _userRepository;

    public GetUserHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<ApplicationUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User with Id {request.Id} not found.");
        }

        return user;
    }
}
