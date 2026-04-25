using FluentValidation;
using Identity.Models;
using Identity.Services;
using MediatR;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IValidator<DeleteUserCommand> _validator;
    private readonly IApplicationUserRepository _appliationUserRepository;
    private readonly IIdentityService _identityService;

    public DeleteUserHandler(
        IValidator<DeleteUserCommand> validator,
        IApplicationUserRepository appliationUserRepository,
        IIdentityService identityService)
    {
        _validator = validator;
        _appliationUserRepository = appliationUserRepository;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        var user = await _appliationUserRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User with {request.Id} not found.");
        }

        var isAdmin = _identityService.GetUserRoles().Contains(UserRoles.Admin);
        var isOwner = request.Id.ToString() == _identityService.GetUserId();

        if (!isOwner && !isAdmin)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this user.");
        }

        _appliationUserRepository.Delete(user);
        await _appliationUserRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
