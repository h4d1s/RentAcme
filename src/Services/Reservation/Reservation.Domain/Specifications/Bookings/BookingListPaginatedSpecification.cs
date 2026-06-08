using Ardalis.Specification;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Reservation.Domain.Specifications.Bookings;

public class BookingListPaginatedSpecification : Specification<Booking>
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public string? Order { get; }
    public string? OrderBy { get; }
    public List<Guid>? UserIds { get; }
    public Guid? VehicleId { get; }
    public DateTime? PickupDate { get; }
    public DateTime? ReturnDate { get; }
    public BookingStatus? Status { get; }

    public BookingListPaginatedSpecification(
        int? pageNumber,
        int? pageSize,
        string? order,
        string? orderBy,
        List<Guid> userIds,
        Guid? vehicleId,
        DateTime? pickupDate,
        DateTime? returnDate,
        BookingStatus? status)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Order = order;
        OrderBy = orderBy;
        UserIds = userIds;
        VehicleId = vehicleId;
        PickupDate = pickupDate;
        ReturnDate = returnDate;
        Status = status;

        Query.Where(i => i.VehicleId == vehicleId, vehicleId.HasValue);
        Query.Where(i => userIds.Contains(i.UserId), userIds.Count > 0);
        Query.Where(i => i.PickupDate >= pickupDate, pickupDate.HasValue);
        Query.Where(i => i.ReturnDate <= returnDate, returnDate.HasValue);
        Query.Where(i => i.Status == status, status.HasValue);

        if (!string.IsNullOrEmpty(orderBy))
        {
            var isDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            switch (orderBy.ToLower())
            {
                case "pickupdate":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.PickupDate);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.PickupDate);
                    }
                    break;
                case "returndate":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.ReturnDate);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.ReturnDate);
                    }
                    break;
                case "status":
                    if (!isDesc)
                    {
                        Query.OrderBy(x => x.Status);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Status);
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
            UserIds != null && UserIds.Any()
                ? string.Join("-", UserIds.OrderBy(id => id))
                : "no-users",
            VehicleId?.ToString() ?? "no-vehicle",
            PickupDate?.ToString("o") ?? "no-pickup",
            ReturnDate?.ToString("o") ?? "no-return",
            Status?.ToString() ?? "no-status"
        };
        var rawKey = string.Join(":", keyParts);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        return $"booking:v{version}:list:count:{hash}";
    }
}
