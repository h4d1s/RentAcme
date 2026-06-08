using Common.Models;
using MediatR;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.Features.Users.Queries.GetUserList;

public class GetUserListQuery : IRequest<PagedResponse<ApplicationUser>>
{
    public string? Order { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
