using Identity.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservation.API.IntegrationTests.Fakes;

public sealed class FakeIdentityTokenService : IIdentityTokenService
{
    public Task<string> GetValidTokenAsync()
    {
        return Task.FromResult("fake-token");
    }
}
