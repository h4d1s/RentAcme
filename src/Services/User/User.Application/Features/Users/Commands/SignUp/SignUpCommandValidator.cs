using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;

namespace User.Application.Features.Users.Commands.SignUp;

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(p => p.UserName)
            .NotEmpty();

        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty()
            .Equal(p => p.Password)
                .WithMessage("Passwords must match.");

        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(p => p.FirstName)
            .NotEmpty();

        RuleFor(p => p.LastName)
            .NotEmpty();

        RuleFor(p => p.PhoneNumber)
            .NotEmpty();

        RuleFor(p => p.Street)
            .NotEmpty();

        RuleFor(p => p.City)
            .NotEmpty();

        RuleFor(p => p.PostalCode)
            .NotEmpty();

        RuleFor(p => p.Country)
            .NotEmpty();
    }
}
