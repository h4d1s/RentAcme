using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Domain.Specifications.Vehicles;

public class VehicleFilterCountSpecification : Specification<Vehicle>
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

    public VehicleFilterCountSpecification(
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
