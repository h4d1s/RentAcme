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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVehicleHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VehicleResponse> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(request.Id);

        if (vehicle == null)
        {
            throw new NotFoundException($"Vehicle with Id {request.Id} not found.");
        }

        var vehicleResponse = _mapper.Map<VehicleResponse>(vehicle);

        return vehicleResponse;
    }
}
