using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Inventory.Domain.AggregatesModel.BrandAggregate;

namespace Inventory.Infrastructure.Persistence.EntityConfigurations;

public class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder
            .Ignore(b => b.DomainEvents);

        builder
            .HasIndex(b => b.Name)
            .IsUnique();

        builder
            .HasMany(e => e.Models)
            .WithOne(e => e.Brand)
            .HasForeignKey(e => e.BrandId)
            .IsRequired();
    }
}
