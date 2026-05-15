using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;
using Identity.Models;
using Identity.Services;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Commands;

public class ReserveBookingCommandHandlerTests
{
    private Mock<IBookingRepository> _mockBookingRepository;
    private Mock<IValidator<ReserveBookingCommand>> _mockValidator;
    private Mock<IUserGrpcClientService> _mockUserClientGrpcService;
    private Mock<IInventoryGrpcClientService> _mockInventoryGrpcService;
    private Mock<IIdentityService> _mockIdentityService;
    private ReserveBookingHandler _handler;

    public ReserveBookingCommandHandlerTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockValidator = new Mock<IValidator<ReserveBookingCommand>>();
        _mockUserClientGrpcService = new Mock<IUserGrpcClientService>();
        _mockInventoryGrpcService = new Mock<IInventoryGrpcClientService>();
        _mockIdentityService = new Mock<IIdentityService>();

        _handler = new ReserveBookingHandler(
            _mockBookingRepository.Object,
            _mockValidator.Object,
            _mockUserClientGrpcService.Object,
            _mockInventoryGrpcService.Object,
            _mockIdentityService.Object);
    }

    [Fact]
    public async Task Handle_UnauthorizedUserReserving_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string? currentUserId = null;
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeCommand = new ReserveBookingCommand {
            UserId = userId, 
            VehicleId = vehicleId,
            PickupDate = pickupDate,
            ReturnDate = returnDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        // Act
        async Task Act() => await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(Act);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeCommand = new ReserveBookingCommand
        {
            UserId = userId,
            VehicleId = vehicleId,
            PickupDate = pickupDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] {
                new ValidationFailure("ReturnDate", "Return date does not exist.")
            }));

        // Act
        async Task Act() => await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithPermissionReservingAndOwner_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeCommand = new ReserveBookingCommand { 
            UserId = userId,
            VehicleId = vehicleId,
            PickupDate = pickupDate,
            ReturnDate = returnDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { Permissions.Bookings.Reserve });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking.Id, result);
    }

    [Fact]
    public async Task Handle_UserWithPermissionReservingWithDifferentUserIdInCommand_ThrowsAuthenticationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeCommand = new ReserveBookingCommand {
            UserId = differentCustomerId,
            VehicleId = vehicleId,
            PickupDate = pickupDate,
            ReturnDate = returnDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { Permissions.Bookings.Reserve });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        async Task Act() => await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithAllPermissionsReserving_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeCommand = new ReserveBookingCommand {
            UserId = userId,
            VehicleId = vehicleId,
            PickupDate = pickupDate,
            ReturnDate = returnDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { 
                Permissions.Bookings.Reserve,
                Permissions.Bookings.ViewAny
            });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking.Id, result);
    }

    [Fact]
    public async Task Test_UserWithAllPermissionReservingWithDifferentUserIdInCommand_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeCommand = new ReserveBookingCommand { 
            UserId = differentCustomerId,
            VehicleId = vehicleId,
            PickupDate = pickupDate,
            ReturnDate = returnDate
        };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> {
                Permissions.Bookings.Reserve,
                Permissions.Bookings.ViewAny
            });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var handler = new ReserveBookingHandler(
            _mockBookingRepository.Object,
            _mockValidator.Object,
            _mockUserClientGrpcService.Object,
            _mockInventoryGrpcService.Object,
            _mockIdentityService.Object);

        // Act
        var result = await handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        Assert.Equal(fakeBooking.Id, result);
    }
}
