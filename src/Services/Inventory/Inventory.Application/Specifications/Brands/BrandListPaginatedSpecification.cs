using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BrandAggregate;

namespace Inventory.Application.Specifications.Brands;

public class BrandListPaginatedSpecification : Specification<Brand>
{
    public BrandListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy)
    {
        if (!string.IsNullOrEmpty(orderBy))
        {
            var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            switch (orderBy.ToLower())
            {
                case "name":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Name);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Name);
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
}
