using Reservation.Application.Features.Bookings.Commands.ReserveBooking;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public class GetBookingValidator : AbstractValidator<GetBookingQuery>
{
    public GetBookingValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty.");
    }
}
