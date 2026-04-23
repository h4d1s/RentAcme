using Reservation.Domain.AggregatesModel.BookingAggregate;
using Common.Models;

namespace Reservation.Application.Features.Bookings.Queries.GetBookingList;

public record GetBookingListQuery : IRequest<PagedResponse<Booking>>
{
    public List<Guid>? UserIds { get; set; } = new List<Guid>();
    public Guid? VehicleId { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BookingStatus? Status { get; set; }

    public string? Order { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
