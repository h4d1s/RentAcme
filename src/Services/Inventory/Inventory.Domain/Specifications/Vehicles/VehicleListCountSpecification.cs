using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Domain.Specifications.Vehicles;

public class VehicleListCountSpecification : Specification<Vehicle>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }
    public decimal? RentalPricePerDayFrom { get; }
    public decimal? RentalPricePerDayTo { get; }

    public VehicleListCountSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy,
        decimal? rentalPricePerDayFrom,
        decimal? rentalPricePerDayTo)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Order = order;
        OrderBy = orderBy;
        RentalPricePerDayFrom = rentalPricePerDayFrom;
        RentalPricePerDayTo = rentalPricePerDayTo;
    }

    public string GetRedisCacheKey(long version)
    {
        var keyParts = new List<string>
        {
            PageNumber?.ToString() ?? "1",
            PageSize?.ToString() ?? "10",
            Order ?? "asc",
            OrderBy ?? "id",
            RentalPricePerDayFrom?.ToString() ?? "any",
            RentalPricePerDayTo?.ToString() ?? "any"
        };
        var rawKey = string.Join(":", keyParts);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        return $"vehicles:v{version}:list:count:{hash}";
    }
}
