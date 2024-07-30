using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Exceptions;
using User.Application.Features.Users.Commands.SignIn;
using User.Application.Infrastructure.Services;

namespace User.Application.Features.Users.Commands.UserInfo;

public class UserInfoCommandHandler : IRequestHandler<UserInfoCommand, UserInfoResponse>
{
    private readonly IUserService _userService;
    private readonly IValidator<UserInfoCommand> _validator;

    public UserInfoCommandHandler(
        IUserService userService,
        IValidator<UserInfoCommand> validator)
    {
        _userService = userService;
        _validator = validator;
    }

    public async Task<UserInfoResponse> Handle(UserInfoCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var claims = await _userService.GenerateClaimsAsync(request.Id);

        return new UserInfoResponse { Claims = claims };
    }
}
