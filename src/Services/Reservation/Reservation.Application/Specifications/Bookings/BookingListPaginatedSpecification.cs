using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Specifications.Bookings;

public class BookingListPaginatedSpecification : Specification<Booking>
{
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
        Query.Where(i => i.VehicleId == vehicleId, vehicleId.HasValue);
        Query.Where(i => userIds.Contains(i.UserId), userIds.Count > 0);
        Query.Where(i => i.PickupDate >= pickupDate, pickupDate.HasValue);
        Query.Where(i => i.ReturnDate <= returnDate, returnDate.HasValue);
        Query.Where(i => i.Status == status, status.HasValue);


        if (!string.IsNullOrEmpty(orderBy))
        {
            switch (orderBy.ToLower())
            {
                case "pickupdate":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        Query.OrderBy(x => x.PickupDate);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.PickupDate);
                    }
                    break;
                case "returndate":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        Query.OrderBy(x => x.ReturnDate);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.ReturnDate);
                    }
                    break;
                case "status":
                    if (string.IsNullOrEmpty(order) || order.ToLower() == "desc")
                    {
                        Query.OrderBy(x => x.Status);
                    }
                    else
                    {
                        Query.OrderByDescending(x => x.Status);
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

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            Query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
    }
}
