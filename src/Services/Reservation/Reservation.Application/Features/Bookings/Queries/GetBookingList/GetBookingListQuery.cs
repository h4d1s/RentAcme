using MediatR;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Reservation.Application.Features.Bookings.Queries.GetBookingList;

public record GetBookingListQuery : IRequest<PagedResponse<Booking>>
{
    public Guid? userId { get; set; }
    public Guid? vehicleId { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BookingStatus? Status { get; set; }

    public string? Order { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
