using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Application.Specifications.Vehicles;

public class VehicleListPaginatedSpecification : Specification<Vehicle>
{
    public VehicleListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy,
        decimal? rentalPricePerDayFrom,
        decimal? rentalPricePerDayTo)
    {
        var querySpec = Query;

        querySpec
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue);

        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "brand":
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
                    {
                        querySpec.OrderBy(x => x.Variant.Model.Brand.Name);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.Variant.Model.Brand.Name);
                    }
                    break;
                case "model":
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
                    {
                        querySpec.OrderBy(x => x.Variant.Model.Name);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.Variant.Model.Name);
                    }
                    break;
                case "variant":
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
                    {
                        querySpec.OrderBy(x => x.Variant.Name);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.Variant.Name);
                    }
                    break;
                case "price":
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
                    {
                        querySpec.OrderBy(x => x.RentalPricePerDay);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.RentalPricePerDay);
                    }
                    break;
                case "power":
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
                    {
                        querySpec.OrderBy(x => x.Variant.Power);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.RentalPricePerDay);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(order) || order.ToLower() != "desc")
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

        querySpec
            .Include(o => o.Variant)
            .Include(o => o.Variant.Model.Brand);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            querySpec
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
