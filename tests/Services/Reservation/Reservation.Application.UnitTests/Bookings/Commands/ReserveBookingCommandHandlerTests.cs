using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using GrpcIntegrationHelpers.ClientServices;
using Moq;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.UnitTests.Bookings.Commands
{
    public class ReserveBookingCommandHandlerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IValidator<ReserveBookingCommand>> _mockValidator;
        private Mock<IMapper> _mockMapper;
        private Mock<IInventoryGrpcClientService> _mockInventoryGrpcService;

        public ReserveBookingCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<ReserveBookingCommand>>();
            _mockMapper = new Mock<IMapper>();
            _mockInventoryGrpcService = new Mock<IInventoryGrpcClientService>();
        }

        [Fact]
        public async Task Test_ValidInput_BookingId()
        {
            // Arrange
            var fakeCommand = new ReserveBookingCommand();
            var fakeBooking = new Booking(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow);

            _mockValidator
                .Setup(p => p.ValidateAsync(fakeCommand, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockUnitOfWork
                .Setup(p => p.BookingRepository.AddAsync(It.IsAny<Booking>()))
                .ReturnsAsync(fakeBooking.Id);

            var handler = new ReserveBookingHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockValidator.Object,
                _mockInventoryGrpcService.Object);

            // Act
            var result = await handler.Handle(fakeCommand, CancellationToken.None);

            // Assert
            Assert.Equal(fakeBooking.Id, result);
        }
    }
}
