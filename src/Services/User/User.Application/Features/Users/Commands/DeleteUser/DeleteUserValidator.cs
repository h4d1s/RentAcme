using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("A valid user ID is required.");
    }
}
