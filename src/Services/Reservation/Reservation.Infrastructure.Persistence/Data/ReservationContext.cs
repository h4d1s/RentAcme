using Microsoft.EntityFrameworkCore;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Infrastructure.Persistence.EntityConfigurations;

namespace Reservation.Infrastructure.Persistence.Data;

public class ReservationContext : DbContext
{
    public ReservationContext(DbContextOptions<ReservationContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookingEntityConfiguration());
    }
}
