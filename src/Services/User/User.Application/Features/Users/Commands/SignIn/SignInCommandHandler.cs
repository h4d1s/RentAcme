using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Exceptions;
using User.Application.Infrastructure.Services;

namespace User.Application.Features.Users.Commands.SignIn;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponse>
{
    private readonly IUserService _userService;
    private readonly IValidator<SignInCommand> _validator;

    public SignInCommandHandler(
        IUserService userService,
        IValidator<SignInCommand> validator)
    {
        _userService = userService;
        _validator = validator;
    }

    public async Task<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var result = await _userService.SignInAsync(request.Username, request.Password, request.Scopes);

        return result;
    }
}
