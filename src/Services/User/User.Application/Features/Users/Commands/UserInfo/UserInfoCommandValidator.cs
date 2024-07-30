using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Infrastructure.Services;

namespace User.Application.Features.Users.Commands.UserInfo;

public class UserInfoCommandValidator : AbstractValidator<UserInfoCommand>
{
    private readonly IUserService _userService;

    public UserInfoCommandValidator(
        IUserService userService)
    {
        _userService = userService;

        RuleFor(p => p.Id)
            .NotEmpty()
            .MustAsync(UserIdMustExist)
            .WithMessage("{PropertyName} does not exist.");
    }

    private async Task<bool> UserIdMustExist(Guid id, CancellationToken arg2)
    {
        return await _userService.ExistsAsync(id);
    }
}
