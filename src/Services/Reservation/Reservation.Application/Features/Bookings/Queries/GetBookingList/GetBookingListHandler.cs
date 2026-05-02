using Common.Models;
using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Reservation.Application.Exceptions;
using Reservation.Application.Specifications.Bookings;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Queries.GetBookingList;

public class GetBookingListHandler : IRequestHandler<GetBookingListQuery, PagedResponse<Booking>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserGrpcClientService _userGrpcClientService;

    public GetBookingListHandler(
        IBookingRepository bookingRepository,
        IIdentityService identityService,
        IUserGrpcClientService userGrpcClientService)
    {
        _bookingRepository = bookingRepository;
        _identityService = identityService;
        _userGrpcClientService = userGrpcClientService;
    }

    public async Task<PagedResponse<Booking>> Handle(GetBookingListQuery request, CancellationToken cancellationToken)
    {
        var filterUserIds = new List<Guid>();
        var currentUserId = _identityService.GetUserId() ?? throw new BadRequestException("User not authenticated.");
        var currentRoles = _identityService.GetUserRoles() ?? throw new BadRequestException("User role not found.");

        var user = await _userGrpcClientService.GetUserByExternalIdAsync(currentUserId);
        if (user == null)
        {
            throw new BadRequestException("User not found.");
        }

        var permissions = _identityService.GetUserPermissions();

        if (!permissions.Contains(Permissions.Bookings.ViewAny))
        {
            filterUserIds = new List<Guid> { user.Id };
        }
        else
        {
            filterUserIds = request.UserIds ?? new List<Guid>();
        }

        var specification = new BookingListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy,
            filterUserIds,
            request.VehicleId,
            request.PickupDate,
            request.ReturnDate,
            request.Status);
        var bookingList = await _bookingRepository.ListAsync(specification);

        specification = new BookingListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy,
            filterUserIds,
            request.VehicleId,
            request.PickupDate,
            request.ReturnDate,
            request.Status);
        var bookingListAllCount = await _bookingRepository.CountAsync(specification);

        return new PagedResponse<Booking>(
            request.Page,
            request.PageSize,
            bookingListAllCount,
            bookingList
        );
    }
}
