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

    public ReserveBookingCommandHandlerTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockValidator = new Mock<IValidator<ReserveBookingCommand>>();
        _mockUserClientGrpcService = new Mock<IUserGrpcClientService>();
        _mockInventoryGrpcService = new Mock<IInventoryGrpcClientService>();
        _mockIdentityService = new Mock<IIdentityService>();
    }

    [Fact]
    public async Task Test_UnauthorizedUserReserving_ThrowsAuthenticationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = string.Empty;
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.Now.AddDays(1);
        var returnDate = DateTime.Now.AddDays(3);
        var fakeCommand = new ReserveBookingCommand { UserId = userId, VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string>());

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto());

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        var handler = new ReserveBookingHandler(
            _mockBookingRepository.Object,
            _mockValidator.Object,
            _mockUserClientGrpcService.Object,
            _mockInventoryGrpcService.Object,
            _mockIdentityService.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            handler.Handle(fakeCommand, CancellationToken.None));
    }

    [Fact]
    public async Task Test_CustomerReserving_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.Now.AddDays(1);
        var returnDate = DateTime.Now.AddDays(3);
        var fakeCommand = new ReserveBookingCommand { UserId = userId, VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { UserRoles.Customer });

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

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

    [Fact]
    public async Task Test_CustomerReservingWithDifferentUserIdInCommand_ThrowsAuthenticationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.Now.AddDays(1);
        var returnDate = DateTime.Now.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeCommand = new ReserveBookingCommand { UserId = differentCustomerId, VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { UserRoles.Customer });

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

        var handler = new ReserveBookingHandler(
            _mockBookingRepository.Object,
            _mockValidator.Object,
            _mockUserClientGrpcService.Object,
            _mockInventoryGrpcService.Object,
            _mockIdentityService.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthenticationException>(() =>
            handler.Handle(fakeCommand, CancellationToken.None));
    }

    [Fact]
    public async Task Test_AdminReserving_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.Now.AddDays(1);
        var returnDate = DateTime.Now.AddDays(3);
        var fakeCommand = new ReserveBookingCommand { UserId = userId, VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { UserRoles.Admin });

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

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

    [Fact]
    public async Task Test_AdminReservingWithDifferentUserIdInCommand_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.Now.AddDays(1);
        var returnDate = DateTime.Now.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeCommand = new ReserveBookingCommand { UserId = differentCustomerId, VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockBookingRepository
            .Setup(p => p.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync(fakeBooking.Id);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockIdentityService
            .Setup(x => x.GetUserId())
            .Returns(currentUserId);
        _mockIdentityService
            .Setup(x => x.GetUserPermissions())
            .Returns(new List<string> { Permissions.Bookings.Reserve, Permissions.Bookings.ViewAny });

        _mockUserClientGrpcService
            .Setup(s => s.GetUserByExternalIdAsync(currentUserId))
            .ReturnsAsync(new UserDto { Id = userId, ExternalId = currentUserId });

        _mockInventoryGrpcService
            .Setup(s => s.GetVehicleAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleDto { RentalPricePerDay = 100m });

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
