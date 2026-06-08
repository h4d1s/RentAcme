using Ardalis.Specification;
using System.Security.Cryptography;
using System.Text;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Domain.Specifications.ApplicationUsers;

public class ApplicationUserListPaginatedSpecification : Specification<ApplicationUser>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }

    public ApplicationUserListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Order = order;
        OrderBy = orderBy;

        if (!string.IsNullOrEmpty(orderBy))
        {
            var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            switch (orderBy.ToLower())
            {
                case "firstName":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.FirstName);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.FirstName);
                    }
                    break;
                case "lastName":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.LastName);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.LastName);
                    }
                    break;
                default:
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Id);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Id);
                    }
                    break;
            }
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var page = Math.Max(pageNumber.Value, 1);
            Query
                .Skip((page - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
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
        return $"users:v{version}:list:{hash}";
    }
}
