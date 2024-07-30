using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.ApplicationUser;

namespace User.Application.Features.Users.Commands.SignIn;

public record SignInResponse
{
    public ClaimsIdentity Identity { get; set; } = null!;
}
