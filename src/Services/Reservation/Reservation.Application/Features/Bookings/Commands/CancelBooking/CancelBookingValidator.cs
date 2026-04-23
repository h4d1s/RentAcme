using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;

namespace Reservation.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingValidator : AbstractValidator<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public CancelBookingValidator(
        IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;

        RuleFor(p => p.BookingId)
            .NotEmpty()
            .MustAsync(BookingIdMustExist)
            .WithMessage("{PropertyName} does not exist.");
    }

    private async Task<bool> BookingIdMustExist(Guid id, CancellationToken arg2)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        return booking is not null;
    }
}
