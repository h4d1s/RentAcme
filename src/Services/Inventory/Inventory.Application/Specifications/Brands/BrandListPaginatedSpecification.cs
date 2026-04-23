using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Specifications.Brands;

public class BrandListPaginatedSpecification : Specification<Brand>
{
    public BrandListPaginatedSpecification(
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
