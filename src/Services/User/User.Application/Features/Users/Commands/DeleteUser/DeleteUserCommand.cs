using MediatR;

namespace User.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
