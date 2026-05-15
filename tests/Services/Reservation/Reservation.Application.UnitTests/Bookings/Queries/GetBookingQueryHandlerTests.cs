using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;
using Identity.Models;
using Identity.Services;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using Reservation.Application.Features.Bookings.Queries.GetBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Queries;

public class GetBookingQueryHandlerTests
{
    private readonly Mock<IValidator<GetBookingQuery>> _mockValidator;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IUserGrpcClientService> _mockUserGrpcClientService;
    private readonly GetBookingHandler _handler;

    public GetBookingQueryHandlerTests()
    {
        _mockValidator = new Mock<IValidator<GetBookingQuery>>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockIdentityService = new Mock<IIdentityService>();
        _mockUserGrpcClientService = new Mock<IUserGrpcClientService>();

        _handler = new GetBookingHandler(
            _mockValidator.Object,
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
        var fakeQuery = new GetBookingQuery
        {
            Id = fakeBooking.Id
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        // Act
        async Task Act() => await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(Act);
    }

    [Fact]
    public async Task Handle_InvalidQuery_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeQuery = new GetBookingQuery();

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] {
                new ValidationFailure("Id", "Id does not exist.")
            }));

        // Act
        async Task Act() => await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithPermissionQueryAndOwner_Booking()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeQuery = new GetBookingQuery
        {
            Id = fakeBooking.Id
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
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
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking, result);
    }

    [Fact]
    public async Task Handle_UserWithPermissionQueryAndNotOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeBooking = new Booking(vehicleId, differentCustomerId, pickupDate, returnDate);
        var fakeQuery = new GetBookingQuery
        {
            Id = fakeBooking.Id
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
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
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        async Task Act() => await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithAllPermissionsQueryOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeQuery = new GetBookingQuery
        {
            Id = fakeBooking.Id
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> {
                Permissions.Bookings.ViewOwn,
                Permissions.Bookings.ViewAny
            });

        _mockUserGrpcClientService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking, result);
    }

    [Fact]
    public async Task Handle_UserWithAllPermissionsQueryAndNotOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeBooking = new Booking(vehicleId, differentCustomerId, pickupDate, returnDate);
        var fakeQuery = new GetBookingQuery
        {
            Id = fakeBooking.Id
        };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeQuery, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> {
                Permissions.Bookings.ViewOwn,
                Permissions.Bookings.ViewAny
            });

        _mockUserGrpcClientService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeQuery, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking, result);
    }
}
