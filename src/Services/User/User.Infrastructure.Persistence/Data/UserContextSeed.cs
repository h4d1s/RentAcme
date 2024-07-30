using User.Domain.AggregatesModel.ApplicationUser;
using User.Infrastructure.Persistence;
using User.Infrastructure.Persistence.Data;
using Persistence;
using User.Application.Infrastructure.Services;

namespace User.Application.Infrastructure.Persistence;

public class UserContextSeed : IDbSeeder<UserContext>
{
    private readonly IUserService _userService;

    public UserContextSeed(
        IUserService userService)
    {
        _userService = userService;
    }

    public async Task SeedAsync(UserContext context)
    {
        if (await _userService.AnyAsync()
            && await _userService.AnyRolesAsync())
        {
            return;   // DB has been seeded
        }

        await _userService.CreateRoleAsync(_userService.GetAdminRoleName());
        await _userService.CreateRoleAsync(_userService.GetCustomerRoleName());

        var adminAddress = new Address
        {
            Street = "5476a",
            City = "London",
            Country = "UK",
            PostalCode = "12345"
        };
        ApplicationUser adminUser = new ApplicationUser()
        {
            UserName = "admin",
            Email = "admin@mail.com",
            EmailConfirmed = true,
            PhoneNumber = "+112347895",
            FirstName = "Jim",
            LastName = "Smith",
            Address = adminAddress
        };
        await _userService.CreateAsync(adminUser, "Passw0rd!");
        await _userService.AddUserToRolesAsync(adminUser, new List<string> { _userService.GetAdminRoleName() });
        await _userService.AddClaimsAsync(adminUser, new List<string> { _userService.GetAdminRoleName() }, adminAddress);

        var customerAddress = new Address
        {
            Street = "Grande Rue 42",
            City = "London",
            Country = "UK",
            PostalCode = "23456"
        };
        ApplicationUser customerUser = new ApplicationUser()
        {
            UserName = "customer",
            Email = "customer@mail.com",
            EmailConfirmed = true,
            PhoneNumber = "+125478965",
            FirstName = "David",
            LastName = "Gray",
            Address = customerAddress
        };
        await _userService.CreateAsync(customerUser, "Passw0rd!");
        await _userService.AddUserToRolesAsync(adminUser, new List<string> { _userService.GetCustomerRoleName() });
        await _userService.AddClaimsAsync(customerUser, new List<string> { _userService.GetCustomerRoleName() }, customerAddress);
    }
}
