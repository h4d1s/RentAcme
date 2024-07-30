using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    IBookingRepository BookingRepository { get; }
    Task<int> SaveEntitiesAsync(CancellationToken cancellationToken);
}
