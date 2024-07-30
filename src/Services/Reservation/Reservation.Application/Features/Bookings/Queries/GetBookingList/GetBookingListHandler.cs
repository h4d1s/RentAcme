using Common.Models;
using LinqKit;
using MediatR;
using Reservation.Application.Exceptions;
using Reservation.Application.Specifications.Bookings;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Queries.GetBookingList;

public class GetBookingListHandler : IRequestHandler<GetBookingListQuery, PagedResponse<Booking>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookingListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<Booking>> Handle(GetBookingListQuery request, CancellationToken cancellationToken)
    {
        var specification = new BookingListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy,
            request.userId,
            request.vehicleId,
            request.PickupDate,
            request.ReturnDate,
            request.Status);
        var bookingList = await _unitOfWork.BookingRepository.ListAsync(specification);

        specification = new BookingListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy,
            request.userId,
            request.vehicleId,
            request.PickupDate,
            request.ReturnDate,
            request.Status);
        var bookingListAllCount = await _unitOfWork.BookingRepository.CountAsync(specification);

        return new PagedResponse<Booking>(
            request.Page,
            request.PageSize,
            bookingListAllCount,
            bookingList
        );
    }
}
