using Inventory.Application.Specifications;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Specifications.Bookings
{
    public class BookingListPaginatedSpecification : BaseSpecification<Booking>
    {
        public BookingListPaginatedSpecification(
            int? pageNumber,
            int? pageSize,
            string? order,
            string? orderBy,
            Guid? userId,
            Guid? vehicleId,
            DateTime? pickupDate,
            DateTime? returnDate,
            BookingStatus? status)
        : base(i =>
            (!vehicleId.HasValue || i.VehicleId == vehicleId) &&
            (!userId.HasValue || i.UserId == userId) &&
            (!pickupDate.HasValue || i.PickupDate >= pickupDate) &&
            (!returnDate.HasValue || i.ReturnDate <= returnDate) &&
            (!status.HasValue || i.Status == status)
        )
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                var ordering = "";
                switch (orderBy.ToLower())
                {
                    case "pickupdate":
                        ordering = "PickupDate";
                        break;
                    case "returndate":
                        ordering = "ReturnDate";
                        break;
                    case "status":
                        ordering = "Status";
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

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
            }
        }
    }
}
