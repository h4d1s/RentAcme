using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.AggregatesModel.ApplicationUser.Events;

public class UserCreatedDomainEvent : INotification
{
    public ApplicationUser User { get; private set; }

    public UserCreatedDomainEvent(ApplicationUser user)
    {
        User = user;
    }
}