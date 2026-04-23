using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Infrastructure.Persistence.EntityConfigurations;

public class BookingEntityConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder
            .Ignore(b => b.DomainEvents);
        builder
            .Property(b => b.Price)
            .HasColumnType("decimal(10, 2)")
            .HasPrecision(10, 2);
    }
}
