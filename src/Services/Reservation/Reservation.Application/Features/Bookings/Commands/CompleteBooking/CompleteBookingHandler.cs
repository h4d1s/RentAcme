using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Commands.CompleteBooking;

public class CompleteBookingHandler : IRequestHandler<CompleteBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IValidator<CompleteBookingCommand> _validator;
    private readonly IUserGrpcClientService _userGrpcClientService;
    private readonly IIdentityService _identityService;

    public CompleteBookingHandler(
        IBookingRepository bookingRepository,
        IValidator<CompleteBookingCommand> validator,
        IUserGrpcClientService userGrpcClientService,
        IIdentityService identityService)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _userGrpcClientService = userGrpcClientService ?? throw new ArgumentNullException(nameof(userGrpcClientService));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid complete booking request", validationResult);
        }

        var bookingToUpdate = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (bookingToUpdate is null)
        {
            throw new NotFoundException($"Booking with {request.BookingId} not found.");
        }

        var currentUserId = _identityService.GetUserId() ?? throw new BadRequestException("User not authenticated.");
        var currentRoles = _identityService.GetUserRoles() ?? throw new BadRequestException("User role not found.");

        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId);
        if (user == null)
        {
            throw new BadRequestException("User not found.");
        }

        var isOwner = bookingToUpdate.UserId == user.Id;
        var isAdmin = currentRoles.Contains(UserRoles.Admin);

        if (!isOwner && !isAdmin)
        {
            throw new AuthenticationException("You are not authorized to reserve this booking.");
        }

        bookingToUpdate.SetCompleteStatus();
        _bookingRepository.Update(bookingToUpdate);
        await _bookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
