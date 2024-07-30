using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Exceptions;
using User.Application.Infrastructure.Services;
using User.Application.Models;

namespace User.Application.Features.Users.Commands.SignUp;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public SignUpCommandHandler(
        IUserService userService,
        IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var validator = new SignUpCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var signUpRequest = _mapper.Map<SignUpRequest>(request);

        await _userService.SignUpAsync(signUpRequest);

        return Unit.Value;
    }
}
