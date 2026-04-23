using MassTransit.DependencyInjection;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Queries.GetUser;

public record GetUserQuery : IRequest<ApplicationUser>
{
    public Guid Id { get; set; }
}