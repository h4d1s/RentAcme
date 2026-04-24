using Inventory.Domain.AggregatesModel.BrandAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
