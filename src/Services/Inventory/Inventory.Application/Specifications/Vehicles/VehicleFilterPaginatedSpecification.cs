using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Application.Specifications.Vehicles;

public class VehicleFilterPaginatedSpecification : Specification<Vehicle>
{
    public VehicleFilterPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy,
        Gearbox? gearbox,
        Category? category,
        decimal? rentalPricePerDayFrom,
        decimal? rentalPricePerDayTo,
        DateTime? pickupDate,
        DateTime? returnDate)
    {
        Query
            .Where(i => i.Variant.Gearbox == gearbox, gearbox.HasValue)
            .Where(i => i.Variant.Model.Category == category, category.HasValue)
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue)
            .Where(i => !i.IsLocked);

        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "price":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        Query.OrderBy(x => x.RentalPricePerDay);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.RentalPricePerDay);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
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
            .Include($"{nameof(Vehicle.Variant)}.{nameof(Variant.Model)}.{nameof(Model.Brand)}");

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            Query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
