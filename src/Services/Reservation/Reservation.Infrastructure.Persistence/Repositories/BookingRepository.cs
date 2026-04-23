using Ardalis.Specification.EntityFrameworkCore;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Infrastructure.Persistence.Data;

namespace Reservation.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ReservationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Booking>> ListAsync(Specification<Booking> spec)
    {
        return await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public async Task<int> CountAsync(Specification<Booking> spec)
    {
        return await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }
}
