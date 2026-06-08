using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Domain.Specifications.Vehicles;

public class VehicleFilterPaginatedSpecification : Specification<Vehicle>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }
    public Gearbox? Gearbox { get; }
    public Category? Category { get; }
    public decimal? RentalPricePerDayFrom { get; }
    public decimal? RentalPricePerDayTo { get; }
    public DateTime? PickupDate { get; }
    public DateTime? ReturnDate { get; }

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
        PageNumber = pageNumber;
        PageSize = pageSize;
        Order = order;
        OrderBy = orderBy;
        Gearbox = gearbox;
        Category = category;
        RentalPricePerDayFrom = rentalPricePerDayFrom;
        RentalPricePerDayTo = rentalPricePerDayTo;
        PickupDate = pickupDate;
        ReturnDate = returnDate;

        Query
            .Where(i => i.Variant.Gearbox == gearbox, gearbox.HasValue)
            .Where(i => i.Variant.Model.Category == category, category.HasValue)
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue)
            .Where(i => i.Status == VehicleStatus.Available);

        if (!string.IsNullOrEmpty(orderBy))
        {
            var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            switch (orderBy.ToLower())
            {
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
            .Include($"{nameof(Vehicle.Variant)}.{nameof(Variant.Model)}.{nameof(Model.Brand)}");

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var page = Math.Max(pageNumber.Value, 1);
            Query
                .Skip((page - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }

    public string GetRedisCacheKey(long version)
    {
        var keyParts = new List<string>
        {
            PageNumber?.ToString() ?? "1",
            PageSize?.ToString() ?? "10",
            Order ?? "asc",
            OrderBy ?? "id",
            Gearbox?.ToString() ?? "any",
            Category?.ToString() ?? "any",
            RentalPricePerDayFrom?.ToString() ?? "any",
            RentalPricePerDayTo?.ToString() ?? "any",
            PickupDate?.ToString("o") ?? "any",
            ReturnDate?.ToString("o") ?? "any"
        };
        var rawKey = string.Join(":", keyParts);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        return $"vehicles:v{version}:search:count:{hash}";
    }
}
