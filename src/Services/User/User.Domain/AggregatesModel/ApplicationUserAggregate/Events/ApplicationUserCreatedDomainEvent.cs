using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate.Events;

public class ApplicationUserCreatedDomainEvent : INotification
{
    public ApplicationUser User { get; private set; }

    public ApplicationUserCreatedDomainEvent(ApplicationUser user)
    {
        User = user;
    }
}