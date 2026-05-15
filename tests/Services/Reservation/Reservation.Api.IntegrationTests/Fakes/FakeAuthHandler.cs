using Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Reservation.API.IntegrationTests.Fakes;

public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user"),
            new Claim("sub", "test-user"),
            new Claim("permissions", Permissions.Bookings.ViewOwn),
            new Claim("permissions", Permissions.Bookings.ViewAny),
            new Claim("permissions", Permissions.Bookings.Cancel),
            new Claim("permissions", Permissions.Bookings.Reserve),
            new Claim("permissions", Permissions.Bookings.Complete),
            new Claim("aud", "rent-acme-aud")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
