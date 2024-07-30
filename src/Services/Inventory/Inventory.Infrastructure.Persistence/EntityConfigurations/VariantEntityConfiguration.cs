using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Inventory.Domain.AggregatesModel.VariantAggreate;

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
