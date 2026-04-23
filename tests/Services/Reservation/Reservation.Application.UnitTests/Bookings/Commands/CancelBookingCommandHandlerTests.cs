using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using Identity.Models;
using Identity.Services;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Commands;

public class CancelBookingCommandHandlerTests
{
    private Mock<IBookingRepository> _mockBookingRepository;
    private Mock<IValidator<CancelBookingCommand>> _mockValidator;
    private Mock<IUserGrpcClientService> _mockUserGrpcService;
    private Mock<IIdentityService> _mockIdentityService;

    public CancelBookingCommandHandlerTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockValidator = new Mock<IValidator<CancelBookingCommand>>();
        _mockUserGrpcService = new Mock<IUserGrpcClientService>();
        _mockIdentityService = new Mock<IIdentityService>();
    }

    [Fact]
    public async Task Test_ValidInput()
    {
        // Arrange
        var fakeBooking = FakeBooking();
        var userId = Guid.NewGuid();
        var fakeCommand = new CancelBookingCommand { BookingId = fakeBooking.Id };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(default))
            .ReturnsAsync(true);

        _mockIdentityService
            .Setup(x => x.GetUserId()).Returns(userId.ToString());
        _mockIdentityService
            .Setup(x => x.GetUserRoles()).Returns(new List<string> { UserRoles.Customer });

        var handler = new CancelBookingHandler(
            _mockBookingRepository.Object,
            _mockUserGrpcService.Object,
            _mockValidator.Object,
            _mockIdentityService.Object);

        // Act
        var result = await handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        var responseBooking = await _mockBookingRepository.Object.GetByIdAsync(fakeCommand.BookingId);
        Assert.Equal(BookingStatus.Canceled, responseBooking?.Status);
    }

    [Fact]
    public async Task Test_InvalidInput()
    {
        // Arrange
        var fakeBooking = FakeBooking();
        var userId = Guid.NewGuid();
        var fakeCommand = new CancelBookingCommand { BookingId = Guid.NewGuid() };

        _mockValidator
            .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] {
                new ValidationFailure("Id", "Id does not exist.")
            }));

        _mockBookingRepository
            .Setup(p => p.GetByIdAsync(fakeBooking.Id))
            .ReturnsAsync(fakeBooking);
        _mockBookingRepository
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(default))
            .ReturnsAsync(true);

        _mockIdentityService
            .Setup(x => x.GetUserId()).Returns(userId.ToString());
        _mockIdentityService
            .Setup(x => x.GetUserRoles()).Returns(new List<string> { UserRoles.Customer });

        var handler = new CancelBookingHandler(
            _mockBookingRepository.Object,
            _mockUserGrpcService.Object,
            _mockValidator.Object,
            _mockIdentityService.Object);

        // Act
        async Task Act() => await handler.Handle(fakeCommand, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BadRequestException>(Act);
    }

    private Booking FakeBooking()
    {
        return new Booking(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
    }
}
