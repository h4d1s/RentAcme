using MediatR;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public class GetBookingHandler : IRequestHandler<GetBookingQuery, Booking>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookingHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Booking> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(request.Id);

        if (booking == null)
        {
            throw new NotFoundException($"Booking with {request.Id} not found.");
        }

        return booking;
    }
}
