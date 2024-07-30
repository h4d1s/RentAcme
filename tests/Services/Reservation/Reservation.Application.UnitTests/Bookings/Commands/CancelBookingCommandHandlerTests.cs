using FluentValidation;
using FluentValidation.Results;
using Moq;
using Reservation.Application.Exceptions;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.UnitTests.Bookings.Commands
{
    public class CancelBookingCommandHandlerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IValidator<CancelBookingCommand>> _mockValidator;

        public CancelBookingCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<CancelBookingCommand>>();
        }

        [Fact]
        public async Task Test_ValidInput()
        {
            // Arrange
            var fakeCommand = new CancelBookingCommand { BookingId = Guid.NewGuid() };

            _mockValidator
                .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockUnitOfWork
                .Setup(p => p.BookingRepository.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(FakeBooking());

            var handler = new CancelBookingHandler(_mockUnitOfWork.Object, _mockValidator.Object);

            // Act
            var result = await handler.Handle(fakeCommand, CancellationToken.None);

            // Assert
            var responseBooking = await _mockUnitOfWork.Object.BookingRepository.GetByIdAsync(fakeCommand.BookingId);
            Assert.Equal(BookingStatus.Canceled, responseBooking?.Status);
        }

        [Fact]
        public async Task Test_InvalidInput()
        {
            // Arrange
            var fakeCommand = new CancelBookingCommand { BookingId = Guid.NewGuid() };

            _mockValidator
                .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
                .ReturnsAsync(new ValidationResult(new[] {
                    new ValidationFailure("Id", "Id does not exist.")
                }));

            var handler = new CancelBookingHandler(_mockUnitOfWork.Object, _mockValidator.Object);

            // Act
            async Task Act() => await handler.Handle(fakeCommand, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<BadRequestException>(Act);
        }

        private Booking FakeBooking()
        {
            return new Booking(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}
