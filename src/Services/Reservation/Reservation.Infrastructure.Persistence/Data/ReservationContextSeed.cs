
using Persistence;

namespace Reservation.Infrastructure.Persistence.Data;

public class ReservationContextSeed : IDbSeeder<ReservationContext>
{
    public ReservationContextSeed()
    {
    }

    public Task SeedAsync(ReservationContext context)
    {
        return Task.CompletedTask;
    }
}
