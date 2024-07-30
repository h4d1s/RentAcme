using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Application.Features.Users.Commands.UserInfo;

public record UserInfoCommand : IRequest<UserInfoResponse>
{
    public Guid Id { get; set; }
}
