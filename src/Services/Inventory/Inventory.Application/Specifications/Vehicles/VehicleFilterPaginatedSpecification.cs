using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MassTransit.Transports;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Specifications.Vehicles;

public class VehicleFilterPaginatedSpecification : BaseSpecification<Vehicle>
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
            : base(i =>
                (!gearbox.HasValue || i.Variant.Gearbox == gearbox) &&
                (!category.HasValue || i.Variant.Model.Category == category) &&
                (!rentalPricePerDayFrom.HasValue || i.RentalPricePerDay == rentalPricePerDayFrom) &&
                (!rentalPricePerDayTo.HasValue || i.RentalPricePerDay == rentalPricePerDayTo) &&
                ((!pickupDate.HasValue && !returnDate.HasValue) || !i.Bookings.Any(x =>
                    x.PickupDate <= returnDate &&
                    x.ReturnDate >= pickupDate &&
                    x.Status == BookingStatus.Reserved))
            )
    {
        if (!string.IsNullOrEmpty(orderBy))
        {
            var ordering = "";
            switch (orderBy.ToLower())
            {
                case "price":
                    ordering = "RentalPricePerDay";
                    break;
                default:
                    ordering = "Id";
                    break;
            }

            var orderLinq = "asc";
            if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
            {
                orderLinq = "desc";
            }

            ApplyOrderBy(ordering + " " + orderLinq);
        }

        AddInclude(o => o.Bookings);
        AddInclude(o => o.Variant);
        AddInclude($"{nameof(Vehicle.Variant)}.{nameof(Variant.Model)}.{nameof(Model.Brand)}");

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
