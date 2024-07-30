using FluentValidation;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Commands.CompleteBooking;

public class CompleteBookingValidator : AbstractValidator<CompleteBookingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteBookingValidator(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(p => p.BookingId)
            .NotEmpty()
            .MustAsync(BookingIdMustExist)
            .WithMessage("{PropertyName} does not exist.");
    }

    private async Task<bool> BookingIdMustExist(Guid id, CancellationToken arg2)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        return booking != null;
    }
}
