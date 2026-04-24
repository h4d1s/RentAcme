using MediatR;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Queries.GetUser;

public record GetUserQuery : IRequest<ApplicationUser>
{
    public Guid Id { get; set; }
}