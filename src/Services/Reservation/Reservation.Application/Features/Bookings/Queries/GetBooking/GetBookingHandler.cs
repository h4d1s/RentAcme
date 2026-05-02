using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public class GetBookingHandler : IRequestHandler<GetBookingQuery, Booking>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserGrpcClientService _userGrpcClientService;

    public GetBookingHandler(
        IBookingRepository bookingRepository,
        IIdentityService identityService,
        IUserGrpcClientService userGrpcClientService)
    {
        _bookingRepository = bookingRepository;
        _identityService = identityService;
        _userGrpcClientService = userGrpcClientService;
    }

    public async Task<Booking> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Id);

        if (booking is null)
        {
            throw new NotFoundException($"Booking with {request.Id} not found.");
        }

        var currentUserId = _identityService.GetUserId() ?? throw new BadRequestException("User not authenticated.");
        var currentRoles = _identityService.GetUserRoles() ?? throw new BadRequestException("User role not found.");

        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId);
        if (user == null)
        {
            throw new BadRequestException("User not found.");
        }

        var permissions = _identityService.GetUserPermissions();
        var isOwner = booking.UserId == user.Id;
        var canView = isOwner || permissions.Contains(Permissions.Bookings.ViewAny);

        if (!canView)
        {
            throw new AuthenticationException("You are not authorized to view this booking.");
        }

        return booking;
    }
}
