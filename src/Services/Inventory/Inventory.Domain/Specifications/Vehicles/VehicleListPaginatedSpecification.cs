using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Inventory.Domain.Specifications.Vehicles;

public class VehicleListPaginatedSpecification : Specification<Vehicle>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }
    public decimal? RentalPricePerDayFrom { get; }
    public decimal? RentalPricePerDayTo { get; }

    public VehicleListPaginatedSpecification(
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
        
        Query
            .Where(i => i.RentalPricePerDay >= rentalPricePerDayFrom, rentalPricePerDayFrom.HasValue)
            .Where(i => i.RentalPricePerDay <= rentalPricePerDayTo, rentalPricePerDayTo.HasValue)
            .Where(i => i.Status == VehicleStatus.Available);

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

    public string GetRedisCacheKey(long version)
    {
        var keyParts = new List<string>
        {
            PageNumber?.ToString() ?? "1",
            PageSize?.ToString() ?? "10",
            Order ?? "asc",
            OrderBy ?? "id",
            RentalPricePerDayFrom?.ToString() ?? "any",
            RentalPricePerDayTo?.ToString() ?? "any",
        };
        var rawKey = string.Join(":", keyParts);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        return $"vehicles:v{version}:list:{hash}";
    }
}
