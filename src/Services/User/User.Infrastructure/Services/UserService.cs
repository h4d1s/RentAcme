using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Exceptions;
using User.Domain.AggregatesModel.ApplicationUser;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using User.Application.Models;
using User.Application.Features.Users.Commands.SignIn;
using User.Application.Features.Users.Commands.SignUp;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OpenIddict.Server.AspNetCore;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using User.Application.Infrastructure.Services;
using OpenIddict.Core;
using MassTransit.Internals;
using MediatR;

namespace User.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOpenIddictScopeManager _openIddictScopeManager;
    private readonly IMediator _mediator;
    private const string _adminRoleName = "Admin";
    private const string _customerRoleName = "Customer";

    public UserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IOpenIddictScopeManager openIddictScopeManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _openIddictScopeManager = openIddictScopeManager;
        _mediator = mediator;
    }

    public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
    {
        return await _signInManager.CreateUserPrincipalAsync(user);
    }

    public async Task<Dictionary<string, object>> GenerateClaimsAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        var principal = await CreateUserPrincipalAsync(user);

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [Claims.Subject] = await _userManager.GetUserIdAsync(user)
        };

        if (principal.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = await _userManager.GetEmailAsync(user);
            claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
        }

        if (principal.HasScope(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
            claims[Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
        }

        if (principal.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await _userManager.GetRolesAsync(user);
        }

        if (principal.HasScope(Scopes.Address) && user.Address != null)
        {
            claims[Claims.StreetAddress] = user.Address.Street;
            claims[Claims.Country] = user.Address.Country;
            claims[Claims.PostalCode] = user.Address.PostalCode;
            claims[Claims.Locality] = user.Address.City;
        }

        return claims;
    }

    public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<string>? roles, Address? address)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
        };

        if (roles != null)
        {
            roles.ToList().ForEach(x => claims.Add(new Claim(ClaimTypes.Role, x)));
        }

        if (address != null)
        {
            claims.AddRange(new List<Claim> {
                new Claim(ClaimTypes.StreetAddress, address.Street),
                new Claim(ClaimTypes.Locality, address.City),
                new Claim(ClaimTypes.PostalCode, address.PostalCode),
                new Claim(ClaimTypes.Country, address.Country)
            });
        }

        var result = await _userManager.AddClaimsAsync(user, claims);
        if (!result.Succeeded)
        {
            throw new Exception("Cannot add claims");
        }
    }

    public async Task<bool> CreateRoleAsync(string role)
    {
        var newRole = new IdentityRole(role);
        var result = await _roleManager.CreateAsync(newRole);

        return result.Succeeded;
    }

    public async Task<bool> CreateAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return false;
        }

        var domainEvents = user.Entity.DomainEvents;
        foreach (var domainEvent in domainEvents) {
            await _mediator.Publish(domainEvent);
        }
        user.Entity.ClearDomainEvents();        

        return true;
    }

    public async Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var result = await _userManager.AddToRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            // Handle error
        }
    }

    public async Task<SignInResponse> SignInAsync(string username, string password, IEnumerable<string> scopes)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            false
        );

        if (!result.Succeeded)
        {
            throw new AuthenticationException("Credentials not valid");
        }

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))])
                .SetClaims(Claims.Audience, ["resource_server"]);

        identity.SetScopes(new[]
        {
            Scopes.OpenId,
            Scopes.Email,
            Scopes.Profile,
            Scopes.Roles
        }.Intersect(scopes));

        var resources = await _openIddictScopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync();
        identity.SetResources(resources);

        identity.SetDestinations(GetDestinations);

        return new SignInResponse { Identity = identity };
    }

    public async Task SignUpAsync(SignUpRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new BadRequestException("Password does not match!");
        }

        var newUserAddress = new Address
        {
            City = request.City,
            Street = request.Street,
            PostalCode = request.PostalCode,
            Country = request.Country,
        };
        var newUser = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Address = newUserAddress,
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            throw new BadRequestException("User registration failed!");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        await _userManager.AddToRoleAsync(user, "User");
    }

    public async Task SignOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(Guid id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user != null;
    }

    public async Task<bool> ExistsAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }

    public async Task<bool> AnyAsync()
    {
        return await _userManager.Users.AnyAsync();
    }

    public async Task<bool> AnyRolesAsync()
    {
        return await _roleManager.Roles.AnyAsync();
    }

    public string GetAdminRoleName()
    {
        return _adminRoleName;
    }

    public string GetCustomerRoleName()
    {
        return _customerRoleName;
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
