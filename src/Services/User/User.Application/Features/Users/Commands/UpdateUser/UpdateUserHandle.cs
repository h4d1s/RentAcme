using FluentValidation;
using Identity.Models;
using Identity.Services;
using MediatR;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IApplicationUserRepository _appliationUserRepository;
    private readonly IIdentityService _identityService;

    public UpdateUserHandler(
        IValidator<UpdateUserCommand> validator,
        IApplicationUserRepository appliationUserRepository,
        IIdentityService identityService)
    {
        _validator = validator;
        _appliationUserRepository = appliationUserRepository;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        var user = await _appliationUserRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"{nameof(ApplicationUser)} not found with Id {request.Id}");
        }

        var permissions = _identityService.GetUserPermissions();
        var isOwner = request.Id.ToString() == _identityService.GetUserId();
        var canUpdate = isOwner || permissions.Contains(Permissions.Users.UpdateAny);

        if (!canUpdate)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this user.");
        }

        user.UpdateEmail(request.Email);
        user.UpdateUserName(request.UserName);
        user.UpdateFirstName(request.FirstName);
        user.UpdateLastName(request.LastName);
        user.UpdatePhoneNumber(request.PhoneNumber);
        user.UpdateAddress(request.Street, request.City, request.PostalCode, request.Country);

        _appliationUserRepository.Update(user);
        await _appliationUserRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
