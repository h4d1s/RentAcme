using Persistence;

namespace Reservation.Infrastructure.Persistence.Data;

public class ReservationDbContextSeed : IDbSeeder<ReservationDbContext>
{
    public ReservationDbContextSeed()
    {
    }

    public Task SeedAsync(ReservationDbContext context)
    {
        return Task.CompletedTask;
    }
}
