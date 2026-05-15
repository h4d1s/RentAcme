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
        Query
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue)
            .Where(i => !i.IsLocked);

        if (!string.IsNullOrEmpty(orderBy))
        {
            var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            switch (orderBy.ToLower())
            {
                case "brand":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Variant.Model.Brand.Name);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Variant.Model.Brand.Name);
                    }
                    break;
                case "model":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Variant.Model.Name);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Variant.Model.Name);
                    }
                    break;
                case "variant":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Variant.Name);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Variant.Name);
                    }
                    break;
                case "price":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.RentalPricePerDay);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.RentalPricePerDay);
                    }
                    break;
                case "power":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Variant.Power);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Variant.Power);
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

        Query
            .Include(o => o.Variant)
            .Include(o => o.Variant.Model.Brand);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            Query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
