using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Infrastructure.Persistence.Data;

namespace Reservation.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ReservationContext context) : base(context)
    {
    }
}
