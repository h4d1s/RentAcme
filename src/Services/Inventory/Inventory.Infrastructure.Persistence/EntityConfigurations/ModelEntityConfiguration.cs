using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Inventory.Domain.AggregatesModel.ModelAggregate;

namespace Inventory.Infrastructure.Persistence.EntityConfigurations;

public class ModelEntityConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder
            .Ignore(b => b.DomainEvents);

        builder
            .HasMany(e => e.Variants)
            .WithOne(e => e.Model)
            .HasForeignKey(e => e.ModelId)
            .IsRequired();
    }
}
