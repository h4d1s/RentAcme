using EventBus.Services;
using GrpcIntegrationHelpers.ClientServices;
using Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Reservation.API.IntegrationTests.Fakes;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Infrastructure.Persistence.Data;
using Testcontainers.PostgreSql;

namespace Reservation.API.IntegrationTests.Fixtures;

public sealed class BookingControllerFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public PostgreSqlContainer PostgreSqlContainer { get; private set; } = null!;

    private string _connectionString => PostgreSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("reservation_test_db")
            .WithUsername("rent-acme")
            .WithPassword("Passw0rd!")
            .WithPortBinding(5432, true)
            .Build();
        await PostgreSqlContainer.StartAsync(CancellationToken.None);

        // Migrate
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();
        await db.Database.MigrateAsync();
    }

    public async Task<Guid> SeedBookingAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();
        var booking = new Booking(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(3)
        );
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();
        return booking.Id;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["KEYCLOAK_BACKEND_SECRET"] = "test-secret"
                });
            })
            .ConfigureTestServices(services =>
            {
                // DB
                services.RemoveAll(typeof(DbContextOptions<ReservationDbContext>));
                services.AddDbContext<ReservationDbContext>(options =>
                {
                    options.UseNpgsql(_connectionString);
                });

                // Auth
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
                            "Test", _ => { });
                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                        .RequireAuthenticatedUser()
                        .Build();
                });
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                // DI
                services.RemoveAll<IIdentityTokenService>();
                services.AddSingleton<IIdentityTokenService, FakeIdentityTokenService>();

                services.RemoveAll<IUserGrpcClientService>();
                services.AddSingleton<IUserGrpcClientService, FakeUserGrpcClientService>();

                services.RemoveAll<IInventoryGrpcClientService>();
                services.AddSingleton<IInventoryGrpcClientService, FakeInventoryGrpcClientService>();

                services.RemoveAll<IIntegrationEventService>();
                services.AddSingleton<IIntegrationEventService, FakeIntegrationEventService>();
            });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await PostgreSqlContainer.DisposeAsync();
    }
}