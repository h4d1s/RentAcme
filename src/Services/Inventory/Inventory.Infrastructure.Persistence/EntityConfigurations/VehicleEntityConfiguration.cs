using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntityConfigurations;

public class VehicleEntityConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder
            .Ignore(v => v.DomainEvents);

        builder
            .Property(v => v.RentalPricePerDay)
            .HasPrecision(10, 2);
    }
}
