using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BookingAggregate;
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
        var querySpec = Query;

        querySpec
            .Where(i => i.Variant.Gearbox == gearbox, gearbox.HasValue)
            .Where(i => i.Variant.Model.Category == category, category.HasValue)
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue);

        if (pickupDate.HasValue && returnDate.HasValue)
        {
            querySpec.Where(i => !i.Bookings.Any(x =>
                x.Status == BookingStatus.Reserved &&
                x.PickupDate <= returnDate &&
                x.ReturnDate >= pickupDate));
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "price":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        querySpec.OrderBy(x => x.RentalPricePerDay);
                    }
                    else
                    {
                        querySpec.OrderByDescending(x => x.RentalPricePerDay);
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

        querySpec
            .Include(o => o.Bookings)
            .Include(o => o.Variant)
            .Include($"{nameof(Vehicle.Variant)}.{nameof(Variant.Model)}.{nameof(Model.Brand)}");

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            querySpec
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
