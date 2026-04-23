using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
