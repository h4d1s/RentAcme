using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public class GetBookingHandler : IRequestHandler<GetBookingQuery, Booking>
{
    private readonly IValidator<GetBookingQuery> _validator;
    private readonly IBookingRepository _bookingRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserGrpcClientService _userGrpcClientService;

    public GetBookingHandler(
        IValidator<GetBookingQuery> validator,
        IBookingRepository bookingRepository,
        IIdentityService identityService,
        IUserGrpcClientService userGrpcClientService)
    {
        _validator = validator;
        _bookingRepository = bookingRepository;
        _identityService = identityService;
        _userGrpcClientService = userGrpcClientService;
    }

    public async Task<Booking> Handle(
        GetBookingQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid get booking request", validationResult);
        }

        var currentUserId = _identityService.GetUserId() ?? throw new UnauthorizedException("User not authenticated.");
        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId);
        if (user == null)
        {
            throw new BadRequestException("User not found.");
        }

        var booking = await _bookingRepository.GetByIdAsync(request.Id);
        if (booking is null)
        {
            throw new NotFoundException($"Booking with {request.Id} not found.");
        }

        var permissions = _identityService.GetUserPermissions();
        var isOwner = booking.UserId == user.Id;
        var canView = isOwner || permissions.Contains(Permissions.Bookings.ViewAny);

        if (!canView)
        {
            throw new UnauthorizedException("You are not authorized to view this booking.");
        }

        return booking;
    }
}
