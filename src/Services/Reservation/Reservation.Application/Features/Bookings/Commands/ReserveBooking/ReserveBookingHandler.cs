using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reservation.Application.Exceptions;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcIntegrationHelpers.ClientServices;

namespace Reservation.Application.Features.Bookings.Commands.ReserveBooking;

public class ReserveBookingHandler : IRequestHandler<ReserveBookingCommand, Guid>
{
    private IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<ReserveBookingCommand> _validator;
    private readonly IInventoryGrpcClientService _inventoryService;

    public ReserveBookingHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<ReserveBookingCommand> validator,
        IInventoryGrpcClientService inventoryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
        _inventoryService = inventoryService;
    }

    public async Task<Guid> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid reserve booking request", validationResult);
        }

        var booking = _mapper.Map<Booking>(request);

        var vehicle = await _inventoryService.GetVehicleAsync(booking.VehicleId);
        booking.SetPrice(vehicle.RentalPricePerDay);

        var id = await _unitOfWork.BookingRepository.AddAsync(booking);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return id;
    }
}
