using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.EntityConfigurations;

public class VariantEntityConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder
            .Ignore(v => v.DomainEvents);

        builder
            .HasOne(e => e.Vehicle)
            .WithOne(e => e.Variant)
            .HasForeignKey<Vehicle>(e => e.VariantId)
            .IsRequired();
    }
}
