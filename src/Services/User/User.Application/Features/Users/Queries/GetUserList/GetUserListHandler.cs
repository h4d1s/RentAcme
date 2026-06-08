using Common.Models;
using MediatR;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Domain.Specifications.ApplicationUsers;

namespace User.Application.Features.Users.Queries.GetUserList;

public class GetUserListHandler : IRequestHandler<GetUserListQuery, PagedResponse<ApplicationUser>>
{
    private readonly IApplicationUserRepository _applicationUserRepository;

    public GetUserListHandler(
        IApplicationUserRepository applicationUserRepository)
    {
        _applicationUserRepository = applicationUserRepository;
    }

    public async Task<PagedResponse<ApplicationUser>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var specification = new ApplicationUserListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var bookingList = await _applicationUserRepository.ListAsync(specification);

        var countSpecification = new ApplicationUserListCountPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var bookingListAllCount = await _applicationUserRepository.CountAsync(countSpecification);

        return new PagedResponse<ApplicationUser>(
            request.Page,
            request.PageSize,
            bookingListAllCount,
            bookingList
        );
    }
}
