using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Application.Features.Users.Commands.SignOut;

public record SignOutCommand : IRequest<Unit>
{
}
