using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Commands.ReserveBooking;

public class ReserveBookingHandler : IRequestHandler<ReserveBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IValidator<ReserveBookingCommand> _validator;
    private readonly IUserGrpcClientService _userGrpcClientService;
    private readonly IInventoryGrpcClientService _inventoryGrpcClientService;
    private readonly IIdentityService _identityService;

    public ReserveBookingHandler(
        IBookingRepository bookingRepository,
        IValidator<ReserveBookingCommand> validator,
        IUserGrpcClientService userGrpcClientService,
        IInventoryGrpcClientService inventoryGrpcClientService,
        IIdentityService identityService)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _inventoryGrpcClientService = inventoryGrpcClientService ?? throw new ArgumentNullException(nameof(inventoryGrpcClientService));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _userGrpcClientService = userGrpcClientService ?? throw new ArgumentNullException(nameof(userGrpcClientService));
    }

    public async Task<Guid> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid reserve booking request", validationResult);
        }

        var currentUserId = _identityService.GetUserId() ?? throw new UnauthorizedException("User not authenticated.");
        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId) ??
            throw new BadRequestException("User not found.");

        var isOwner = request.UserId == user.Id;
        var permissions = _identityService.GetUserPermissions();
        var canView = isOwner || permissions.Contains(Permissions.Bookings.ViewAny);

        if (!canView)
        {
            throw new UnauthorizedException("You are not authorized to reserve this booking.");
        }

        var booking = new Booking(request.VehicleId, request.UserId, request.PickupDate, request.ReturnDate);
        var vehicle = await _inventoryGrpcClientService.GetVehicleAsync(booking.VehicleId);
        booking.SetPrice(vehicle.RentalPricePerDay);

        var id = await _bookingRepository.AddAsync(booking);
        await _bookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }
}
