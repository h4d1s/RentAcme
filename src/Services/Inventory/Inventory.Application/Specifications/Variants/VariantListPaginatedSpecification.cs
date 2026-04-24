using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.VariantAggreate;

namespace Inventory.Application.Specifications.Models;

public class VariantListPaginatedSpecification : Specification<Variant>
{
    public VariantListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy)
    {
        var querySpec = Query;

        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "name":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        querySpec.OrderBy(x => x.Name);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.Name);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        querySpec.OrderBy(x => x.Id);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.Id);
                    }
                    break;
            }
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            querySpec
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
