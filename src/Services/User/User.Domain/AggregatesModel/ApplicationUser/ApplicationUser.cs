using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User.Domain.AggregatesModel.ApplicationUser.Events;
using User.Domain.Common;

namespace User.Domain.AggregatesModel.ApplicationUser;

public class ApplicationUser
    : IdentityUser, IAggregateRoot
{
    public readonly Entity Entity = new EntityUser();

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Address? Address { get; set; }

    public ApplicationUser()
    {
        Entity.AddDomainEvent(new UserCreatedDomainEvent(this));
    }

    class EntityUser : Entity { }
}
