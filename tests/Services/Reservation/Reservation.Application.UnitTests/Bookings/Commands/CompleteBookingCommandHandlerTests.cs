using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;
using Identity.Models;
using Identity.Services;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Commands.CompleteBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Commands;

public class CompleteBookingCommandHandlerTests
{
    private Mock<IBookingRepository> _mockBookingRepository;
    private Mock<IValidator<CompleteBookingCommand>> _mockValidator;
    private Mock<IUserGrpcClientService> _mockUserClientGrpcService;
    private Mock<IInventoryGrpcClientService> _mockInventoryGrpcService;
    private Mock<IIdentityService> _mockIdentityService;
    private CompleteBookingHandler _handler;

    public CompleteBookingCommandHandlerTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockValidator = new Mock<IValidator<CompleteBookingCommand>>();
        _mockUserClientGrpcService = new Mock<IUserGrpcClientService>();
        _mockInventoryGrpcService = new Mock<IInventoryGrpcClientService>();
        _mockIdentityService = new Mock<IIdentityService>();

        _handler = new CompleteBookingHandler(
            _mockBookingRepository.Object,
            _mockValidator.Object,
            _mockUserClientGrpcService.Object,
            _mockIdentityService.Object);
    }

    [Fact]
    public async Task Handle_UnauthorizedUserCompleting_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string? currentUserId = null;
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeCommand = new CompleteBookingCommand { BookingId = fakeBooking.Id };

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
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeCommand = new CompleteBookingCommand();

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] {
                new ValidationFailure("BookingId", "Booking id does not exist.")
            }));

        // Act
        async Task Act() => await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(Act);
    }

    [Fact]
    public async Task Handle_UserWithPermissionCompleteNotOwnerCompleting_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var differentCustomerId = Guid.NewGuid();
        var fakeBooking = new Booking(vehicleId, differentCustomerId, pickupDate, returnDate);
        var fakeCommand = new CompleteBookingCommand { BookingId = fakeBooking.Id };

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
            .Returns(new List<string> { Permissions.Bookings.Complete });

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        Assert.Equal(BookingStatus.Completed, fakeBooking.Status);
    }

    [Fact]
    public async Task Handle_UserWithPermissionCompleteOwnerCompleting_BookingId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid().ToString();
        var vehicleId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(3);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);
        var fakeCommand = new CompleteBookingCommand
        {
            BookingId = fakeBooking.Id,
        };

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
                Permissions.Bookings.Complete
            });

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(p => p.UnitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        Assert.Equal(BookingStatus.Completed, fakeBooking.Status);
    }
}
