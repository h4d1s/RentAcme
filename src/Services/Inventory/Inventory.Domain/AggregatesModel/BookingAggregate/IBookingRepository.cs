using Ardalis.Specification;
using Inventory.Domain.Common;

namespace Inventory.Domain.AggregatesModel.BookingAggregate;

public interface IBookingRepository : IRepository<Booking>
{
    public Task<IReadOnlyList<Booking>> ListAsync(Specification<Booking> spec);
    public Task<int> CountAsync(Specification<Booking> spec);
}
