using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System.Reflection.Metadata;

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

        builder
            .HasMany(e => e.Bookings)
            .WithOne(e => e.Vehicle)
            .HasForeignKey(e => e.VehicleId)
            .IsRequired();
    }
}
