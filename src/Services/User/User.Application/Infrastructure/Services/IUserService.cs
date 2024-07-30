using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Application.Features.Users.Commands.SignIn;
using User.Application.Features.Users.Commands.SignUp;
using User.Application.Models;
using User.Domain.AggregatesModel.ApplicationUser;

namespace User.Application.Infrastructure.Services;

public interface IUserService
{
    public string GetAdminRoleName();
    public string GetCustomerRoleName();
    public Task<SignInResponse> SignInAsync(string username, string password, IEnumerable<string> scopes);
    public Task SignUpAsync(SignUpRequest request);
    public Task SignOut();
    public Task<ApplicationUser?> GetByIdAsync(Guid id);
    public Task<bool> ExistsAsync(Guid id);
    public Task<bool> ExistsAsync(string username);
    public Task<bool> AnyAsync();
    public Task<bool> AnyRolesAsync();
    public Task<Dictionary<string, object>> GenerateClaimsAsync(Guid id);
    public Task AddClaimsAsync(ApplicationUser user, IEnumerable<string>? roles, Address? address);
    public Task<bool> CreateAsync(ApplicationUser user, string password);
    public Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    public Task<bool> CreateRoleAsync(string role);
}
