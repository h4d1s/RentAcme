using Reservation.Domain.Common;
using Reservation.Domain.Specifications.Bookings;

namespace Reservation.Domain.AggregatesModel.BookingAggregate;

public interface IBookingRepository : IRepository<Booking>
{
    public Task<IReadOnlyList<Booking>> ListAsync(BookingListPaginatedSpecification spec);
    public Task<int> CountAsync(BookingListCountPaginatedSpecification spec);
}
