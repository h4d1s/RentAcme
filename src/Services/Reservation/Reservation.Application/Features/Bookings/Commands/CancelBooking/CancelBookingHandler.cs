using FluentValidation;
using MediatR;
using Reservation.Application.Exceptions;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CancelBookingCommand> _validator;

    public CancelBookingHandler(
        IUnitOfWork unitOfWork,
        IValidator<CancelBookingCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid cancel booking request", validationResult);
        }

        var bookingToUpdate = await _unitOfWork.BookingRepository.GetByIdAsync(request.BookingId);

        if (bookingToUpdate == null)
        {
            throw new NotFoundException($"Booking with {request.BookingId} not found.");
        }

        bookingToUpdate.SetCanceledStatus();

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
