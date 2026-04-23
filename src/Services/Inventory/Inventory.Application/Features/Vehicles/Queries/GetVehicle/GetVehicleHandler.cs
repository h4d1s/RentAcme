using AutoMapper;
using Inventory.Application.Exceptions;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicle;

public class GetVehicleHandler : IRequestHandler<GetVehicleQuery, VehicleResponse>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehicleHandler(
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VehicleResponse> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);

        if (vehicle is null)
        {
            throw new NotFoundException($"Vehicle with Id {request.Id} not found.");
        }

        var vehicleResponse = _mapper.Map<VehicleResponse>(vehicle);

        return vehicleResponse;
    }
}
