using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserGrpcClientService _userGrpcClientService;
    private readonly IValidator<CancelBookingCommand> _validator;
    private readonly IIdentityService _identityService;

    public CancelBookingHandler(
        IBookingRepository bookingRepository,
        IUserGrpcClientService userGrpcClientService,
        IValidator<CancelBookingCommand> validator,
        IIdentityService identityService)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _userGrpcClientService = userGrpcClientService ?? throw new ArgumentNullException(nameof(userGrpcClientService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid cancel booking request", validationResult);
        }

        var bookingToUpdate = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (bookingToUpdate is null)
        {
            throw new NotFoundException($"Booking with {request.BookingId} not found.");
        }

        var currentUserId = _identityService.GetUserId() ?? throw new BadRequestException("User not authenticated.");

        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId);
        if (user == null)
        {
            throw new BadRequestException("User not found.");
        }

        var isOwner = bookingToUpdate.UserId == user.Id;

        if (!isOwner)
        {
            throw new AuthenticationException("You are not authorized to reserve this booking.");
        }

        bookingToUpdate.SetCanceledStatus();
        _bookingRepository.Update(bookingToUpdate);
        await _bookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
