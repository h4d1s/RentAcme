using Ardalis.Specification;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using System.Security.Cryptography;
using System.Text;

namespace Reservation.Domain.Specifications.Bookings;

public class BookingListCountPaginatedSpecification : Specification<Booking>
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

    public BookingListCountPaginatedSpecification(
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
