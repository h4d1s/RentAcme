using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;
using Identity.Models;
using Identity.Services;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Queries.GetBookingList;
using Reservation.Application.Specifications.Bookings;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Queries;

public class GetBookingListQueryHandlerTests
{
    private readonly Mock<IValidator<GetBookingListQuery>> _mockValidator;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IUserGrpcClientService> _mockUserGrpcClientService;
    private readonly GetBookingListHandler _handler;

    public GetBookingListQueryHandlerTests()
    {
        _mockValidator = new Mock<IValidator<GetBookingListQuery>>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockIdentityService = new Mock<IIdentityService>();
        _mockUserGrpcClientService = new Mock<IUserGrpcClientService>();

        _handler = new GetBookingListHandler(
            _mockBookingRepository.Object,
            _mockIdentityService.Object,
            _mockUserGrpcClientService.Object);
    }

    [Fact]
    public async Task Handle_UnauthorizedUserQuering_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string? currentUserId = null;
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeListQuery = new GetBookingListQuery();

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        // Act
        async Task Act() => await _handler.Handle(fakeListQuery, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithPermissionQueryOwn_PagedResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeListQuery = new GetBookingListQuery();
        var fakeList = new List<Booking>() {
            new Booking(Guid.NewGuid(), userId, pickupDate, returnDate),
            new Booking(Guid.NewGuid(), userId, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(6)),
            new Booking(Guid.NewGuid(), Guid.NewGuid(), pickupDate, DateTime.UtcNow.AddDays(8)),
            new Booking(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(9)),
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeListQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { Permissions.Bookings.ViewOwn });

        _mockUserGrpcClientService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockBookingRepository
            .Setup(p => p.ListAsync(It.IsAny<BookingListPaginatedSpecification>()))
            .ReturnsAsync(fakeList.Where(s => s.UserId == userId).ToList());
        _mockBookingRepository
            .Setup(p => p.CountAsync(It.IsAny<BookingListPaginatedSpecification>()))
            .ReturnsAsync(fakeList.Count(s => s.UserId == userId));

        // Act
        var result = await _handler.Handle(fakeListQuery, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalRecords);
    }

    [Fact]
    public async Task Handle_UserWithPermissionQueryAny_PagedResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeListQuery = new GetBookingListQuery();
        var fakeList = new List<Booking>() {
            new Booking(Guid.NewGuid(), userId, pickupDate, returnDate),
            new Booking(Guid.NewGuid(), userId, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(6)),
            new Booking(Guid.NewGuid(), Guid.NewGuid(), pickupDate, DateTime.UtcNow.AddDays(8)),
            new Booking(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(9)),
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeListQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { Permissions.Bookings.ViewAny });

        _mockUserGrpcClientService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockBookingRepository
            .Setup(p => p.ListAsync(It.IsAny<BookingListPaginatedSpecification>()))
            .ReturnsAsync(fakeList);
        _mockBookingRepository
            .Setup(p => p.CountAsync(It.IsAny<BookingListPaginatedSpecification>()))
            .ReturnsAsync(fakeList.Count);

        // Act
        var result = await _handler.Handle(fakeListQuery, CancellationToken.None);

        // Assert
        Assert.Equal(fakeList, result.Data);
        Assert.Equal(fakeList.Count, result.TotalRecords);
    }
}
