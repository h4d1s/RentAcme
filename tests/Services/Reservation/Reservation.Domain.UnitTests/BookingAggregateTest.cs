using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Exceptions;

namespace Reservation.Domain.UnitTests;

public class BookingAggregateTest
{
    public BookingAggregateTest() { }

    [Fact]
    public void CreateBooking_Success()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(2);

        // Act
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        // Assert
        Assert.NotNull(fakeBooking);
    }

    [Fact]
    public void CreateBooking_ThrowsException()
    {
        // Arrange
        var vehicleId = Guid.Empty;
        var userId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(2);

        // Act
        var act = () => new Booking(vehicleId, userId, pickupDate, returnDate);

        // Assert
        Assert.Throws<ReservationDomainException>(act);
    }

    [Fact]
    public void CreateBooking_RaisesNewEvent()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(2);

        // Act
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        // Assert
        Assert.Single(fakeBooking.DomainEvents!);
    }

    [Fact]
    public void Booking_SetStatusComplete_StatusChangedToComplete()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var returnDate = DateTime.UtcNow.AddDays(2);
        var fakeBooking = new Booking(vehicleId, userId, pickupDate, returnDate);

        // Act
        fakeBooking.SetCompleteStatus();

        // Assert
        Assert.Equal(BookingStatus.Completed, fakeBooking.Status);
    }
}
