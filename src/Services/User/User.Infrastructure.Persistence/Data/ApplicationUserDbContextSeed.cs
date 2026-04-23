using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Domain.Common;
using User.Infrastructure.Persistence;
using User.Infrastructure.Persistence.Data;

namespace User.Application.Infrastructure.Persistence;

public class ApplicationUserDbContextSeed : IDbSeeder<ApplicationUserDbContext>
{
    private readonly ILogger<ApplicationUserDbContextSeed> _logger;

    public ApplicationUserDbContextSeed(
        ILogger<ApplicationUserDbContextSeed> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(ApplicationUserDbContext context)
    {
        if (await context.ApplicationUsers.AnyAsync())
        {
            return;   // DB has been seeded
        }

        try
        {
            var administratorUser = new ApplicationUser(
                "f47ac10b-58cc-4372-a567-0e02b2c3d479",
                "admin@rent-acme.com",
                "rent-acme-admin",
                "Admin",
                "Administrator",
                "+1234567890"
            );
            administratorUser.UpdateAddress("5476a", "London", "12345", "UK");
            await context.ApplicationUsers.AddAsync(administratorUser);

            var customerUser = new ApplicationUser(
                "a1b2c3d4-e5f6-47a8-9b0c-1d2e3f4a5b6c",
                "john.smith@rent-acme.com",
                "jsmith",
                "John",
                "Smith",
                "+1234567890"
            );
            customerUser.UpdateAddress("Grande Rue 42", "London", "23456", "UK");
            await context.ApplicationUsers.AddAsync(customerUser);
            await context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("DB Seeded.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
