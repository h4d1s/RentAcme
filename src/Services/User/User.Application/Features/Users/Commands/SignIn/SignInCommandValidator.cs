using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using User.Application.Infrastructure.Services;

namespace User.Application.Features.Users.Commands.SignIn;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    private readonly IUserService _userService;

    public SignInCommandValidator(
        IUserService userService)
    {
        _userService = userService;

        RuleFor(p => p.Username)
            .NotEmpty()
            .MustAsync(UsernameMustExist)
            .WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.Password)
            .NotEmpty();
    }

    private async Task<bool> UsernameMustExist(string username, CancellationToken arg2)
    {
        return await _userService.ExistsAsync(username);
    }
}
