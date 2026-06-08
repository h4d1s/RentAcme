using Ardalis.Specification;
using System.Security.Cryptography;
using System.Text;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Domain.Specifications.ApplicationUsers;

public class ApplicationUserListCountPaginatedSpecification : Specification<ApplicationUser>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }

    public ApplicationUserListCountPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Order = order;
        OrderBy = orderBy;
    }

    public string GetRedisCacheKey(long version)
    {
        var keyParts = new List<string>
        {
            PageNumber?.ToString() ?? "1",
            PageSize?.ToString() ?? "10",
            Order ?? "asc",
            OrderBy ?? "id"
        };
        var rawKey = string.Join(":", keyParts);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        return $"users:v{version}:list:count:{hash}";
    }
}