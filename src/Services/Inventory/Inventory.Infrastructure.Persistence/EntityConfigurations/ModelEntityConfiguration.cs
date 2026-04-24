using Inventory.Domain.AggregatesModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
