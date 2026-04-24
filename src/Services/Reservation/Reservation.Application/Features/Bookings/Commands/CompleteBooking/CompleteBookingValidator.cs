using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Commands.CompleteBooking;

public class CompleteBookingValidator : AbstractValidator<CompleteBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public CompleteBookingValidator(
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
