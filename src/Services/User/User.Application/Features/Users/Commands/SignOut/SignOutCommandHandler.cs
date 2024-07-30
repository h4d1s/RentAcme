using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Infrastructure.Services;
using User.Application.Models;

namespace User.Application.Features.Users.Commands.SignOut;

public class SignOutCommandHandler : IRequestHandler<SignOutCommand, Unit>
{
    private readonly IUserService _userService;

    public SignOutCommandHandler(
        IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        await _userService.SignOut();

        return Unit.Value;
    }
}
