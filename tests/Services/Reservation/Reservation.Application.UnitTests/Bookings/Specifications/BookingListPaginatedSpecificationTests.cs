using Reservation.Application.Specifications.Bookings;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.UnitTests.Bookings.Specifications;

public class BookingListPaginatedSpecificationTests
{
    private readonly List<Booking> _fakeData;

    public BookingListPaginatedSpecificationTests()
    {
        var vehicleId1 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        _fakeData = new List<Booking>
        {
            new Booking(vehicleId1, Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(2)),
            new Booking(vehicleId1, userId1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)),
            new Booking(Guid.NewGuid(), userId1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(8)),
            new Booking(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(8)),
        };
    }

    [Fact]
    public void FilterByVehicleId()
    {
        // Arrange
        var vehicleId = _fakeData[0].VehicleId;
        var specification = new BookingListPaginatedSpecification(null, null, null, null, [], vehicleId, null, null, null);

        // Act
        var result = specification.Evaluate(_fakeData).ToList();

        // Assert
        Assert.All(result, x => Assert.Equal(vehicleId, x.VehicleId));
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FilterByUserId()
    {
        // Arrange
        var userId = _fakeData[1].UserId;
        var specification = new BookingListPaginatedSpecification(null, null, null, null, [userId], null, null, null, null);

        // Act
        var result = specification.Evaluate(_fakeData).ToList();

        // Assert
        Assert.All(result, x => Assert.Equal(userId, x.UserId));
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Paginate()
    {
        // Arrange
        var specification = new BookingListPaginatedSpecification(2, 1, null, null, [], null, null, null, null);

        // Act
        var result = specification.Evaluate(_fakeData).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(_fakeData[1].Id, result[0].Id);
    }

    [Theory]
    [InlineData("PickupDate", "asc", 0)]
    [InlineData("PickupDate", "desc", 3)]
    public void SortingAscDesc(string orderBy, string order, int expectedFirstDayOffset)
    {
        // Arrange
        var specification = new BookingListPaginatedSpecification(null, null, order, orderBy, [], null, null, null, null);

        // Act
        var result = specification.Evaluate(_fakeData).ToList();

        // Assert
        var expectedDate = DateTime.Now.AddDays(expectedFirstDayOffset).Date;
        Assert.Equal(expectedDate, result.First().PickupDate.Date);
    }

    [Fact]
    public void Range()
    {
        // Arrange
        var specification = new BookingListPaginatedSpecification(null, null, null, null, [], null, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(8), null);

        // Act
        var result = specification.Evaluate(_fakeData).ToList();

        // Assert
        Assert.Single(result);
    }
}
