using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Domain.Specifications.Brands;

public class BrandListCountSpecification : Specification<Brand>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }

    public BrandListCountSpecification(
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
        return $"brands:v{version}:list:count:{hash}";
    }
}
