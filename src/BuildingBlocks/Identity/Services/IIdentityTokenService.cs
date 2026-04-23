using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Services;

public interface IIdentityTokenService
{
    Task<string> GetValidTokenAsync();
}
