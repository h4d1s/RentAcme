using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using User.Domain.AggregatesModel.ApplicationUser;
using User.Infrastructure.Persistence.EntityConfigurations;

namespace User.Infrastructure.Persistence.Data;

public class UserContext : IdentityDbContext<ApplicationUser>
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
}
