using FluentValidation;
using GrpcIntegrationHelpers.ClientServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Commands.ReserveBooking;

public class ReserveBookingValidator : AbstractValidator<ReserveBookingCommand>
{
    private readonly IInventoryGrpcClientService _inventoryService;
    private readonly IUserGrpcClientService _userCheckService;

    public ReserveBookingValidator(
        IInventoryGrpcClientService inventoryService,
        IUserGrpcClientService userCheckService)
    {
        _inventoryService = inventoryService;
        _userCheckService = userCheckService;

        RuleFor(p => p.VehicleId)
            .NotEmpty()
            .MustAsync(VehicleIdMustExist)
            .WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.UserId)
            .NotEmpty()
            .MustAsync(UserIdMustExist)
            .WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.PickupDate)
            .NotEmpty()
            .Must(BeTodayOrFutureDate);

        RuleFor(p => p.ReturnDate)
            .NotEmpty()
            .Must(BeTommorowOrFutureDate)
            .GreaterThanOrEqualTo(p => p.PickupDate.AddDays(1))
            .WithMessage("{PropertyName} must be greater than one day.");
    }

    private async Task<bool> VehicleIdMustExist(Guid id, CancellationToken arg2)
    {
        return await _inventoryService.CheckIfExistsAsync(id);
    }

    private async Task<bool> UserIdMustExist(Guid id, CancellationToken arg2)
    {
        return await _userCheckService.CheckIfExistsAsync(id);
    }

    private bool BeTodayOrFutureDate(DateTime date)
    {
        return date >= DateTime.Today;
    }

    private bool BeTommorowOrFutureDate(DateTime date)
    {
        return date > DateTime.Today;
    }
}
