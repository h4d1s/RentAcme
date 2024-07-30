using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Specifications.Vehicles;

public class VehicleListPaginatedSpecification : BaseSpecification<Vehicle>
{
    public VehicleListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy,
        decimal? rentalPricePerDayFrom,
        decimal? rentalPricePerDayTo)
        : base(i =>
            (!rentalPricePerDayFrom.HasValue || i.RentalPricePerDay >= rentalPricePerDayFrom) &&
            (!rentalPricePerDayTo.HasValue || i.RentalPricePerDay <= rentalPricePerDayTo)
        )
    {
        if (!string.IsNullOrEmpty(orderBy))
        {
            var ordering = "";
            switch (orderBy.ToLower())
            {
                case "brand":
                    ordering = "Variant.Model.Brand.Name";
                    break;
                case "model":
                    ordering = "Variant.Model.Name";
                    break;
                case "variant":
                    ordering = "Variant.Name";
                    break;
                case "price":
                    ordering = "RentalPricePerDay";
                    break;
                case "power":
                    ordering = "";
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

        AddInclude(o => o.Variant);
        AddInclude($"{nameof(Vehicle.Variant)}.{nameof(Variant.Model)}.{nameof(Model.Brand)}");

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
